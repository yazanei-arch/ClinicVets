# tests/test_registration.py
import os
import tempfile

import pytest

from app import app as flask_app, db, User


@pytest.fixture()
def client():
    """
    Fresh isolated SQLite DB for each test (does NOT use your real instance/smarteval.db).
    """
    fd, path = tempfile.mkstemp(suffix=".db")
    os.close(fd)

    flask_app.config.update(
        TESTING=True,
        SQLALCHEMY_DATABASE_URI=f"sqlite:///{path}",
        SQLALCHEMY_TRACK_MODIFICATIONS=False,
        SECRET_KEY="test-secret",
    )

    with flask_app.app_context():
        db.drop_all()
        db.create_all()

        with flask_app.test_client() as client:
            yield client

        db.session.remove()
        db.drop_all()

    try:
        os.remove(path)
    except OSError:
        pass


def post_register(client, **kwargs):
    data = {
        "full_name": kwargs.get("full_name", "Test User"),
        "email": kwargs.get("email", "test@example.com"),
        "role": kwargs.get("role", "student"),
        "password": kwargs.get("password", "12345678"),
        "confirm": kwargs.get("confirm", "12345678"),
    }
    return client.post("/register", data=data, follow_redirects=False)


# ---------------------------
# 1.1: show registration for new user
# (we validate via redirect to /register)
# ---------------------------
def test_home_redirects_to_register(client):
    res = client.get("/", follow_redirects=False)
    assert res.status_code in (301, 302)
    assert "/register" in res.headers.get("Location", "")


# ---------------------------
# 1.1/1.2: registration page loads
# ---------------------------
def test_register_page_loads(client):
    res = client.get("/register")
    assert res.status_code == 200


# ---------------------------
# 1.2: required fields exist (backend validates)
# ---------------------------
def test_register_requires_full_name(client):
    res = post_register(client, full_name="")
    assert res.status_code == 200
    with flask_app.app_context():
        assert User.query.count() == 0


def test_register_passwords_must_match(client):
    res = post_register(client, password="12345678", confirm="87654321")
    assert res.status_code == 200
    with flask_app.app_context():
        assert User.query.count() == 0


# ---------------------------
# 1.3: validate email format + ensure email unique
# ---------------------------
def test_rejects_invalid_email_format(client):
    res = post_register(client, email="not-an-email")
    assert res.status_code == 200
    with flask_app.app_context():
        assert User.query.count() == 0


def test_rejects_duplicate_email(client):
    res1 = post_register(client, email="dup@example.com")
    assert res1.status_code in (301, 302)

    res2 = post_register(client, email="dup@example.com")
    assert res2.status_code == 200

    with flask_app.app_context():
        assert User.query.filter_by(email="dup@example.com").count() == 1


# ---------------------------
# 1.4: password minimum 8 characters
# ---------------------------
def test_rejects_short_password(client):
    res = post_register(client, password="1234567", confirm="1234567")
    assert res.status_code == 200
    with flask_app.app_context():
        assert User.query.count() == 0


# ---------------------------
# 1.5: create user in DB on success
# ---------------------------
def test_creates_user_in_database_on_success(client):
    res = post_register(client, email="create@example.com")
    assert res.status_code in (301, 302)
    with flask_app.app_context():
        u = User.query.filter_by(email="create@example.com").first()
        assert u is not None
        assert u.full_name == "Test User"


# ---------------------------
# 1.6: redirect to login after successful registration
# ---------------------------
def test_success_redirects_to_login(client):
    res = post_register(client, email="redir@example.com")
    assert res.status_code in (301, 302)
    assert "/login" in res.headers.get("Location", "")


# ---------------------------
# 1.7: show error message if registration fails (DB failure)
# We verify: no crash (200) and no user created.
# ---------------------------
def test_registration_db_failure_does_not_crash_and_creates_no_user(client, monkeypatch):
    def boom():
        raise Exception("DB is down")

    monkeypatch.setattr(db.session, "commit", boom)

    res = post_register(client, email="dbfail@example.com")
    assert res.status_code == 200
    with flask_app.app_context():
        assert User.query.filter_by(email="dbfail@example.com").first() is None


# ---------------------------
# 1.9: judge registration email domain allowlist (allowed vs blocked)
# ---------------------------
def test_judge_allowed_domain_is_accepted(client):
    res = post_register(client, email="judge@amazon.com", role="judge")
    assert res.status_code in (301, 302)
    assert "/login" in res.headers.get("Location", "")

    with flask_app.app_context():
        u = User.query.filter_by(email="judge@amazon.com").first()
        assert u is not None
        assert u.role == "judge"


def test_judge_blocked_domain_is_rejected(client):
    res = post_register(client, email="judge@gmail.com", role="judge")
    assert res.status_code == 200

    with flask_app.app_context():
        assert User.query.filter_by(email="judge@gmail.com").first() is None


def test_judge_acil_subdomain_is_accepted(client):
    res = post_register(client, email="judge@cs.huji.ac.il", role="judge")
    assert res.status_code in (301, 302)
    assert "/login" in res.headers.get("Location", "")

    with flask_app.app_context():
        u = User.query.filter_by(email="judge@cs.huji.ac.il").first()
        assert u is not None
        assert u.role == "judge"
