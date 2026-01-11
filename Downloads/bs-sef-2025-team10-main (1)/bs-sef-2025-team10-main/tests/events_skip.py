import pytest
from datetime import datetime, timedelta
from app import app, db, Event, User
from flask_login import login_user

@pytest.fixture
def client():
    app.config["TESTING"] = True
    with app.test_client() as client:
        with app.app_context():
            db.create_all()
        yield client

def login_admin(client):
    with app.app_context():
        admin = User.query.filter_by(email="admin@test.com").first()
        if not admin:
            admin = User(full_name="Admin", email="admin@test.com",
                         password_hash="hashed", role="admin")
            db.session.add(admin)
            db.session.commit()
        login_user(admin)
        return admin

# 5.8 valid future date → expect 201
def test_create_event_valid(client):
    login_admin(client)
    future_date = (datetime.now() + timedelta(days=2)).isoformat()
    res = client.post("/api/events", json={
        "name": "Future Conf",
        "date": future_date,
        "tracks": "AI,Cyber,Cloud",
        "description": "Tech conference"
    })
    assert res.status_code == 201

# 5.9 past date → expect validation error 400
def test_create_event_past_date(client):
    login_admin(client)
    past_date = (datetime.now() - timedelta(days=1)).isoformat()
    res = client.post("/api/events", json={
        "name": "Past Conf",
        "date": past_date,
        "tracks": "AI,Cyber",
        "description": "Invalid"
    })
    assert res.status_code == 400
