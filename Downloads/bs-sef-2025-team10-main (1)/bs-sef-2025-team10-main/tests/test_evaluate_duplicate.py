from werkzeug.security import generate_password_hash
from app import db, User, Project


def test_api_evaluate_rejects_duplicate_submission(client, app):
    # Create a judge + a project in the test DB
    with app.app_context():
        judge = User(
            full_name="Judge One",
            email="judge_dup@test.com",
            role="judge",
            password_hash=generate_password_hash("123")
        )
        db.session.add(judge)
        db.session.commit()

        # Create a student (project needs student_id)
        student = User(
            full_name="Student One",
            email="student_dup@test.com",
            role="student",
            password_hash=generate_password_hash("123")
        )
        db.session.add(student)
        db.session.commit()

        project = Project(
            name="Cyber Project",
            category="Cyber",
            description="Test",
            team_members="A,B",
            contact_email="a@test.com",
            student_id=student.id
        )
        db.session.add(project)
        db.session.commit()

        project_id = project.id
        judge_id = judge.id

    # Log in the judge by setting session (so /api/evaluate passes US7.6 auth)
    with client.session_transaction() as sess:
        sess["user_id"] = judge_id
        sess["role"] = "judge"

    payload = {
        "project_id": project_id,
        "innovation": 7,
        "clarity": 8,
        "completeness": 9,
        "comments": "First submit"
    }

    # First submit should succeed
    r1 = client.post("/api/evaluate", json=payload)
    assert r1.status_code == 200
    assert r1.get_json().get("ok") is True

    # Second submit should fail (duplicate)
    r2 = client.post("/api/evaluate", json=payload)
    assert r2.status_code == 409
    data2 = r2.get_json()
    assert data2.get("ok") is False
