import os
import re
from datetime import datetime, date

from flask import (
    Flask, render_template, request, redirect, url_for,
    flash, session, g, jsonify
)
from flask_sqlalchemy import SQLAlchemy
from sqlalchemy import UniqueConstraint
from sqlalchemy.exc import IntegrityError, SQLAlchemyError
from werkzeug.security import generate_password_hash, check_password_hash
from jinja2 import TemplateNotFound

# Optional: if scoring_service exists
try:
    from scoring_service import (
        fetch_judges_and_projects,
        fetch_current_assignments,
        auto_assign_projects_to_judges,
        save_judge_assignments
    )
except Exception:
    fetch_judges_and_projects = None
    fetch_current_assignments = None
    auto_assign_projects_to_judges = None
    save_judge_assignments = None


# ---------------------------
# App + DB config
# ---------------------------
app = Flask(__name__)
app.config["SECRET_KEY"] = "change-this-secret"  # TODO: change later

os.makedirs(app.instance_path, exist_ok=True)
db_path = os.path.join(app.instance_path, "smarteval.db")

app.config["SQLALCHEMY_DATABASE_URI"] = f"sqlite:///{db_path}"
app.config["SQLALCHEMY_TRACK_MODIFICATIONS"] = False

db = SQLAlchemy(app)


# ---------------------------
# Models
# ---------------------------
class User(db.Model):
    __tablename__ = "users"
    id = db.Column(db.Integer, primary_key=True)
    full_name = db.Column(db.String(120), nullable=False)
    email = db.Column(db.String(255), unique=True, nullable=False, index=True)
    password_hash = db.Column(db.String(255), nullable=False)
    role = db.Column(db.String(50), nullable=False, default="student")  # student, judge, admin


class Event(db.Model):
    __tablename__ = 'events'
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(150), nullable=False)
    date = db.Column(db.DateTime, nullable=False)
    tracks = db.Column(db.String(255), nullable=True)
    description = db.Column(db.Text, nullable=True)


class Project(db.Model):
    __tablename__ = 'projects'
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(150), nullable=False)
    description = db.Column(db.Text, nullable=False)
    category = db.Column(db.String(100), nullable=False)
    team_members = db.Column(db.String(255), nullable=False)
    contact_email = db.Column(db.String(255), nullable=False)
    average_score = db.Column(db.Float, default=0.0)

    status = db.Column(db.String(20), default="Pending")  # Pending, Graded
    feedback = db.Column(db.Text, nullable=True)
    is_published = db.Column(db.Boolean, default=False)

    event_id = db.Column(db.Integer, db.ForeignKey('events.id'), nullable=False)
    student_id = db.Column(db.Integer, db.ForeignKey('users.id'), nullable=False)
    judge_id = db.Column(db.Integer, db.ForeignKey('users.id'), nullable=True)


class Evaluation(db.Model):
    __tablename__ = "evaluations"

    id = db.Column(db.Integer, primary_key=True)
    judge_id = db.Column(db.Integer, db.ForeignKey("users.id"), nullable=False)
    project_id = db.Column(db.Integer, db.ForeignKey("projects.id"), nullable=False)

    innovation = db.Column(db.Integer, nullable=False)
    clarity = db.Column(db.Integer, nullable=False)
    completeness = db.Column(db.Integer, nullable=False)
    comments = db.Column(db.String(500))
    created_at = db.Column(db.DateTime, default=datetime.utcnow, nullable=False)

    __table_args__ = (
        UniqueConstraint("judge_id", "project_id", name="uq_judge_project_once"),
    )


# ---------------------------
# Helpers
# ---------------------------
EMAIL_RE = re.compile(r"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$")
ALLOWED_ROLES = {"student", "judge", "admin"}
ALLOWED_JUDGE_DOMAINS = {
    "ac.il", "microsoft.com", "amazon.com", "intel.com", "ibm.com", "apple.com", "meta.com"
}


def email_domain(email: str) -> str:
    email = (email or "").strip().lower()
    if "@" not in email:
        return ""
    return email.split("@", 1)[1]


def is_allowed_domain(domain: str) -> bool:
    domain = (domain or "").strip().lower()
    return any(domain == d or domain.endswith("." + d) for d in ALLOWED_JUDGE_DOMAINS)


def safe_render(template_name: str, **context):
    try:
        return render_template(template_name, **context)
    except TemplateNotFound:
        return f"Missing template: {template_name}", 200


@app.before_request
def load_logged_in_user():
    user_id = session.get("user_id")
    if not user_id:
        g.user = None
    else:
        g.user = db.session.get(User, user_id)


def require_login():
    if not session.get("user_id"):
        flash("Please login first.", "error")
        return redirect(url_for("login"))
    return None


def login_user(user: User):
    session.clear()
    session["user_id"] = user.id
    session["role"] = user.role


# ---------------------------
# Security Headers (US3.4)
# ---------------------------
@app.after_request
def add_header(response):
    response.headers["Cache-Control"] = "no-cache, no-store, must-revalidate"
    response.headers["Pragma"] = "no-cache"
    response.headers["Expires"] = "0"
    return response


# ---------------------------
# Routes
# ---------------------------
@app.get("/")
def home():
    if session.get("user_id"):
        role = session.get("role")
        if role == "judge":
            return redirect(url_for("judge_dashboard"))
        if role == "admin":
            return redirect(url_for("admin_dashboard"))
        return redirect(url_for("student_dashboard"))
    return redirect(url_for("register"))


# --------- Auth
@app.route("/login", methods=["GET", "POST"])
def login():
    if request.method == "GET":
        return safe_render("login.html", title="Login", page_css="login.css")

    email = request.form.get("email", "").strip().lower()
    password = request.form.get("password", "")

    if not EMAIL_RE.match(email):
        flash("Please enter a valid email.", "error")
        return safe_render("login.html", title="Login", page_css="login.css")

    user = User.query.filter_by(email=email).first()
    if not user or not check_password_hash(user.password_hash, password):
        flash("Invalid email or password.", "error")
        return safe_render("login.html", title="Login", page_css="login.css")

    login_user(user)
    flash(f"Logged in successfully as {user.role}.", "success")
    return redirect(url_for("home"))


@app.route("/logout", methods=["GET", "POST"])
def logout():
    # If you have logout_confirm.html, GET shows confirm, POST does logout.
    if request.method == "GET":
        return safe_render("logout_confirm.html", title="Logout")

    session.clear()
    flash("Logged out successfully.", "success")
    return redirect(url_for("login"))


@app.route("/register", methods=["GET", "POST"])
def register():
    if request.method == "GET":
        return safe_render("register.html", title="Register", page_css="register.css")

    full_name = (request.form.get("full_name") or "").strip()
    email = (request.form.get("email") or "").strip().lower()
    password = request.form.get("password") or ""
    confirm = request.form.get("confirm") or ""
    role = (request.form.get("role") or "student").strip().lower()

    if not full_name:
        flash("Registration failed: full name is required.", "error")
        return safe_render("register.html", title="Register", page_css="register.css")

    if role not in ALLOWED_ROLES:
        flash("Registration failed: invalid role.", "error")
        return safe_render("register.html", title="Register", page_css="register.css")

    if not EMAIL_RE.match(email):
        flash("Registration failed: invalid email format.", "error")
        return safe_render("register.html", title="Register", page_css="register.css")

    if role == "judge" and not is_allowed_domain(email_domain(email)):
        flash("Registration failed: judge email domain is not allowed.", "error")
        return safe_render("register.html", title="Register", page_css="register.css")

    if len(password) < 8:
        flash("Registration failed: password must be at least 8 characters.", "error")
        return safe_render("register.html", title="Register", page_css="register.css")

    if password != confirm:
        flash("Registration failed: passwords do not match.", "error")
        return safe_render("register.html", title="Register", page_css="register.css")

    if User.query.filter_by(email=email).first():
        flash("Registration failed: email already exists.", "error")
        return safe_render("register.html", title="Register", page_css="register.css")

    try:
        user = User(
            full_name=full_name,
            email=email,
            password_hash=generate_password_hash(password),
            role=role
        )
        db.session.add(user)
        db.session.commit()
        flash("Registration successful! Please log in.", "success")
        return redirect(url_for("login"))
    except Exception:
        db.session.rollback()
        flash("Registration failed due to a database error.", "error")
        return safe_render("register.html", title="Register", page_css="register.css")


# --------- Dashboards
@app.get("/dashboard/student")
def student_dashboard():
    auth_check = require_login()
    if auth_check: return auth_check
    
    user_id = session["user_id"]
    user = db.session.get(User, user_id)
    
    # --- FIX: Fetch the student's projects ---
    my_projects = Project.query.filter_by(student_id=user_id).all()
    
    return safe_render("student_dashboard.html", user=user, projects=my_projects)

@app.route("/my-results")
def my_results():
    auth_check = require_login()
    if auth_check: return auth_check
    
    user_id = session["user_id"]
    project = Project.query.filter_by(student_id=user_id).first()
    
    if not project:
        flash("No project found.", "info")
        return redirect(url_for("student_dashboard"))

    # Pass project to template; template handles the "not published" view logic
    return render_template("my_results.html", project=project)

@app.route('/admin/event/<int:event_id>')
def event_details(event_id):
    # Fetch the event object
    event = Event.query.get_or_404(event_id)
    
    # Get all projects linked to this event
    projects = Project.query.filter_by(event_id=event_id).all()
    
    # Get all users who are judges to show in the dropdown (if you have one)
    judges = User.query.filter_by(role='judge').all()
    
    return render_template('event_details.html', event=event, projects=projects, judges=judges)

@app.route('/admin/event/<int:event_id>/add_project', methods=['POST'])
def add_project(event_id):
    # Collect data
    name = request.form.get('name')
    description = request.form.get('description')
    category = request.form.get('category')
    team_members = request.form.get('team_members')
    contact_email = request.form.get('contact_email')
    judge_id = request.form.get('judge_id')

    # Create project
    new_project = Project(
        name=name,
        description=description,
        category=category,
        team_members=team_members,
        contact_email=contact_email,
        event_id=event_id,
        judge_id=judge_id,
        student_id=session.get("user_id") # Admin acting as creator
    )

    try:
        db.session.add(new_project)
        db.session.commit()
        flash("Project successfully added!", "success")
    except Exception as e:
        db.session.rollback()
        flash(f"Error saving project: {str(e)}", "error")

    return redirect(url_for('event_details', event_id=event_id))

@app.get("/dashboard/judge")
def judge_dashboard():
    auth_check = require_login()
    if auth_check:
        return auth_check
    if session.get("role") != "judge":
        flash("Access denied (judge only).", "error")
        return redirect(url_for("home"))

    user = db.session.get(User, session["user_id"])
    projects = Project.query.filter_by(judge_id=user.id).all()
    return safe_render("judge_dashboard.html", title="Judge Dashboard", page_css="judge_dashboard.css", user=user, projects=projects)


@app.get("/dashboard/admin")
def admin_dashboard():
    auth_check = require_login()
    if auth_check:
        return auth_check
    if session.get("role") != "admin":
        flash("Access denied (admin only).", "error")
        return redirect(url_for("home"))

    events = Event.query.order_by(Event.date.asc()).all()
    return safe_render("admin_dashboard.html", title="Admin Dashboard", events=events)


# --------- US5.5 Create Event (POST /api/events)
@app.post("/api/events")
def create_event():
    if not session.get("user_id") or session.get("role") != "admin":
        flash("Error: Could not create event due to unauthorized access.", "error")
        return redirect(url_for("home"))

    name = request.form.get("name")
    date_str = request.form.get("date")
    tracks = request.form.get("tracks")
    description = request.form.get("description")

    if not name or not date_str:
        flash("Error: Could not create event due to missing Name/Date.", "error")
        return redirect(url_for("admin_dashboard"))

    try:
        event_date_obj = datetime.strptime(date_str, "%Y-%m-%d")
        if event_date_obj.date() <= date.today():
            flash("Error: Could not create event due to date not being in the future.", "error")
            return redirect(url_for("admin_dashboard"))

        new_event = Event(
            name=name,
            date=event_date_obj,
            tracks=tracks,
            description=description
        )
        db.session.add(new_event)
        db.session.commit()
        flash(f"Event '{name}' created successfully.", "success")

    except Exception as e:
        db.session.rollback()
        flash(f"Error: Could not create event due to {str(e)}.", "error")

    return redirect(url_for("admin_dashboard"))

@app.route("/assignments")
def assignments_page():
    auth_check = require_login()
    if auth_check: return auth_check
    
    if session.get("role") != "admin":
        flash("Access denied.", "error")
        return redirect(url_for("home"))

    # If scoring service is available, fetch real data. Otherwise, empty.
    judges = []
    projects = []
    if fetch_judges_and_projects:
        try:
            judges, projects = fetch_judges_and_projects()
        except Exception:
            pass

    return safe_render("assignments.html", judges=judges, projects=projects)

@app.route("/assignments/auto-assign", methods=["POST"])
def auto_assign():
    if not auto_assign_projects_to_judges:
        flash("Auto-assign service not available.", "error")
        return redirect(url_for("assignments_page"))

    try:
        judges, projects = fetch_judges_and_projects()
        assignments, loads = auto_assign_projects_to_judges(judges, projects)
        if save_judge_assignments:
            save_judge_assignments(assignments, clear_existing=True)
        flash("Auto-assignment complete.", "success")
    except Exception as e:
        flash(f"Error during assignment: {str(e)}", "error")
        
    return redirect(url_for("assignments_page"))

@app.route("/api/events/<int:event_id>/delete", methods=["POST"])
def delete_event(event_id):
    # 1. Authorization Check
    if not session.get("user_id") or session.get("role") != "admin":
        flash("Error: Unauthorized access.", "error")
        return redirect(url_for("home"))

    # 2. Find the event
    event = Event.query.get_or_404(event_id)

    try:
        # 3. Check for dependencies (prevent deleting events with projects)
        linked_projects = Project.query.filter_by(event_id=event_id).first()
        if linked_projects:
            flash(f"Cannot delete event '{event.name}' because it has projects assigned to it.", "error")
            return redirect(url_for("admin_dashboard"))

        # 4. Delete the event
        db.session.delete(event)
        db.session.commit()
        flash(f"Event '{event.name}' deleted successfully.", "success")
        
    except Exception as e:
        db.session.rollback()
        flash(f"Error deleting event: {str(e)}", "error")

    return redirect(url_for("admin_dashboard"))

@app.route("/api/projects/<int:project_id>/delete", methods=["POST"])
def delete_project(project_id):
    # 1. Authorization Check
    if not session.get("user_id") or session.get("role") != "admin":
        flash("Error: Unauthorized access.", "error")
        return redirect(url_for("home"))

    # 2. Find the project
    project = Project.query.get_or_404(project_id)
    event_id = project.event_id  # Save this so we can redirect back correctly

    try:
        # 3. Delete Project
        db.session.delete(project)
        db.session.commit()
        flash(f"Project '{project.name}' deleted successfully.", "success")
        
    except Exception as e:
        db.session.rollback()
        flash(f"Error deleting project: {str(e)}", "error")

    # 4. Redirect back to the event page
    return redirect(url_for("event_details", event_id=event_id))

# --------- Submit Project
@app.route("/submit-project", methods=["GET", "POST"])
def submit_project():
    auth_check = require_login()
    if auth_check:
        return auth_check
    if session.get("role") != "student":
        flash("Access denied (student only).", "error")
        return redirect(url_for("home"))

    if request.method == "GET":
        return safe_render("submit_project.html", title="Submit Project")

    name = request.form.get("name")
    category = request.form.get("category")
    description = request.form.get("description")
    team_members = request.form.get("team_members") or ""
    contact_email = request.form.get("contact_email") or ""

    if not all([name, category, description]):
        flash("Submission failed: missing fields.", "error")
        return safe_render("submit_project.html", title="Submit Project")

    event = Event.query.first()
    if not event:
        flash("No event exists yet. Admin must create an event first.", "error")
        return redirect(url_for("student_dashboard"))

    new_project = Project(
        name=name,
        category=category,
        description=description,
        team_members=team_members,
        contact_email=contact_email,
        student_id=session["user_id"],
        event_id=event.id,
    )

    try:
        db.session.add(new_project)
        db.session.commit()
        flash("Project submitted successfully!", "success")
        return redirect(url_for("student_dashboard"))
    except Exception as e:
        db.session.rollback()
        flash(f"Submission failed: {str(e)}", "error")
        return safe_render("submit_project.html", title="Submit Project")


# --------- Judge: Evaluation
@app.get("/evaluate/<int:project_id>")
def evaluation_page(project_id):
    auth_check = require_login()
    if auth_check:
        return auth_check
    if session.get("role") != "judge":
        flash("Access denied. Judges only.", "error")
        return redirect(url_for("home"))

    project = Project.query.get_or_404(project_id)
    return safe_render("evaluation.html", title="Evaluate", project=project)


@app.post("/evaluate/<int:project_id>")
def submit_evaluation(project_id):
    auth_check = require_login()
    if auth_check: return auth_check
    
    if session.get("role") != "judge":
        flash("Forbidden.", "error")
        return redirect(url_for("home"))

    project = Project.query.get_or_404(project_id)
    judge_id = session["user_id"]

    # 1. Read Scores
    def read_score(name):
        try:
            v = int(request.form.get(name, 0))
        except Exception:
            return None
        return v if 1 <= v <= 10 else None

    innovation = read_score("innovation")
    clarity = read_score("clarity")
    completeness = read_score("completeness")
    comments = (request.form.get("comments") or "").strip()[:500]

    if innovation is None or clarity is None or completeness is None:
        flash("Scores must be between 1 and 10.", "error")
        return redirect(url_for("evaluation_page", project_id=project.id))

    # 2. Check for Duplicate
    existing = Evaluation.query.filter_by(judge_id=judge_id, project_id=project.id).first()
    if existing:
        flash("You already submitted an evaluation for this project.", "error")
        return redirect(url_for("judge_dashboard"))

    # 3. Save New Evaluation
    ev = Evaluation(
        judge_id=judge_id,
        project_id=project.id,
        innovation=innovation,
        clarity=clarity,
        completeness=completeness,
        comments=comments
    )

    try:
        db.session.add(ev)
        
        # --- FIX: Update Project Status ---
        project.status = "Graded"

        # --- FIX: Calculate New Average Score (US 9 Logic) ---
        # Calculate current submission average
        current_avg = (innovation + clarity + completeness) / 3.0
        
        if project.average_score == 0:
            project.average_score = current_avg
        else:
            # Simple moving average
            project.average_score = (project.average_score + current_avg) / 2.0
            
        project.average_score = round(project.average_score, 2)
        # ----------------------------------------------------

        db.session.commit()
        flash("Evaluation Submitted Successfully!", "success")
        return redirect(url_for("judge_dashboard"))

    except Exception as e:
        db.session.rollback()
        flash(f"Saving evaluation failed: {str(e)}", "error")
        return redirect(url_for("evaluation_page", project_id=project.id))


# --------- API: my projects for judge
@app.get('/api/my-projects')
def api_my_projects():
    if 'user_id' not in session or session.get('role') != 'judge':
        return jsonify({"error": "Unauthorized access"}), 401

    current_judge_id = session.get('user_id')
    my_projects = Project.query.filter_by(judge_id=current_judge_id).all()

    data = []
    for p in my_projects:
        data.append({
            "id": p.id,
            "project_name": p.name,
            "student_name": p.team_members,
            "status": p.status
        })
    return jsonify(data), 200


# --------- US6 Seed
@app.get('/seed-us6')
def seed_us6_data():
    judge = User.query.filter_by(email="judge@test.com").first()
    if not judge:
        judge = User(
            full_name="Judge Ahmed",
            email="judge@test.com",
            role="judge",
            password_hash=generate_password_hash("12345678")
        )
        db.session.add(judge)
        db.session.commit()

    student = User.query.filter_by(email="student@test.com").first()
    if not student:
        student = User(
            full_name="Student Ali",
            email="student@test.com",
            role="student",
            password_hash=generate_password_hash("12345678")
        )
        db.session.add(student)
        db.session.commit()

    event = Event.query.first()
    if not event:
        event = Event(name="Demo Event", date=datetime.utcnow(), tracks="", description="")
        db.session.add(event)
        db.session.commit()

    p1 = Project(
        name="AI Cam",
        category="AI",
        description="Desc",
        team_members="Ali",
        contact_email="ali@test.com",
        student_id=student.id,
        judge_id=judge.id,
        event_id=event.id
    )
    db.session.add(p1)
    db.session.commit()

    return "Data seeded."


# ---------------------------
# Run
# ---------------------------
if __name__ == "__main__":
    with app.app_context():
        db.create_all()
        print("DB:", db_path)
    app.run(debug=True)
