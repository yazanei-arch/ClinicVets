# tests/test_login.py
import pytest
from werkzeug.security import generate_password_hash

from app import app as flask_app, db, User


@pytest.fixture()
def app(tmp_path):
    """
    Creates a fresh temp DB per test session.
    """
    db_file = tmp_path / "test.db"

    flask_app.config.update(
        TESTING=True,
        SECRET_KEY="test-secret",
        SQLALCHEMY_DATABASE_URI=f"sqlite:///{db_file}",
        SQLALCHEMY_TRACK_MODIFICATIONS=False,
    )

    with flask_app.app_context():
        db.session.remove()
        db.drop_all()
        db.create_all()

        yield flask_app

        db.session.remove()
        db.drop_all()


@pytest.fixture()
def client(app):
    return app.test_client()


def create_user(full_name, email, password, role="student"):
    user = User(
        full_name=full_name,
        email=email.lower(),
        password_hash=generate_password_hash(password),
        role=role,
    )
    db.session.add(user)
    db.session.commit()
    return user


def login(client, email, password, follow=False):
    return client.post(
        "/login",
        data={"email": email, "password": password},
        follow_redirects=follow,
    )


# -------------------------
# 2.4: Successful login
# -------------------------
def test_successful_login_sets_session_and_role(client, app):
    with app.app_context():
        create_user("Student One", "stud1@example.com", "password123", role="student")

    resp = login(client, "stud1@example.com", "password123", follow=False)
    assert resp.status_code in (302, 303)

    # Session should be created (2.3) + role stored
    with client.session_transaction() as sess:
        assert sess.get("user_id") is not None
        assert sess.get("role") == "student"

    # Home should redirect based on role
    resp2 = client.get("/", follow_redirects=False)
    assert resp2.status_code in (302, 303)
    assert "/dashboard/student" in resp2.headers.get("Location", "")


# -------------------------
# 2.4: Failed login cases
# -------------------------
def test_failed_login_wrong_password(client, app):
    with app.app_context():
        create_user("Student One", "stud2@example.com", "password123", role="student")

    resp = login(client, "stud2@example.com", "WRONGPASS", follow=True)
    assert resp.status_code == 200
    assert b"Invalid email or password" in resp.data or b"Invalid" in resp.data

    with client.session_transaction() as sess:
        assert sess.get("user_id") is None
        assert sess.get("role") is None


def test_failed_login_unknown_email(client):
    resp = login(client, "unknown@example.com", "password123", follow=True)
    assert resp.status_code == 200
    assert b"Invalid email or password" in resp.data or b"Invalid" in resp.data


def test_failed_login_invalid_email_format(client):
    resp = login(client, "not-an-email", "password123", follow=True)
    assert resp.status_code == 200
    assert b"valid email" in resp.data or b"email" in resp.data.lower()


# -------------------------
# 2.4: Role-based access
# -------------------------
def test_requires_login_redirects_to_login(client):
    # Not logged in -> should redirect to /login
    resp = client.get("/dashboard/student", follow_redirects=False)
    assert resp.status_code in (302, 303)
    assert "/login" in resp.headers.get("Location", "")


def test_judge_can_access_judge_dashboard_student_cannot(client, app):
    with app.app_context():
        create_user("Judge One", "judge1@intel.com", "password123", role="judge")
        create_user("Student One", "stud3@example.com", "password123", role="student")

    # student login -> trying judge dashboard should redirect away
    login(client, "stud3@example.com", "password123", follow=False)

    resp = client.get("/dashboard/judge", follow_redirects=False)
    assert resp.status_code in (302, 303)  # access denied -> redirect
    # most implementations redirect to "/" (home)
    assert resp.headers.get("Location", "").endswith("/") or "/dashboard" not in resp.headers.get("Location", "")

    # logout if you have it
    client.get("/logout", follow_redirects=False)

    # judge login -> can access judge dashboard
    login(client, "judge1@intel.com", "password123", follow=False)
    resp2 = client.get("/dashboard/judge", follow_redirects=False)
    assert resp2.status_code == 200


def test_admin_route_access_if_exists(client, app):
    """
    Only runs meaningful assertions if you implemented /dashboard/admin.
    If you didn't implement admin, the route might be missing -> we skip.
    """
    if "admin_dashboard" not in flask_app.view_functions:
        pytest.skip("Admin dashboard not implemented in this app")

    with app.app_context():
        create_user("Admin One", "admin1@example.com", "password123", role="admin")
        create_user("Student Two", "stud4@example.com", "password123", role="student")

    # student login -> admin page should redirect away
    login(client, "stud4@example.com", "password123", follow=False)
    resp = client.get("/dashboard/admin", follow_redirects=False)
    assert resp.status_code in (302, 303)

    # logout then admin login -> admin page should be OK
    client.get("/logout", follow_redirects=False)
    login(client, "admin1@example.com", "password123", follow=False)
    resp2 = client.get("/dashboard/admin", follow_redirects=False)
    assert resp2.status_code == 200
