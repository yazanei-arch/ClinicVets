import pytest
from datetime import date, timedelta
from app import app, db, Event

# 1. Test Environment Setup (Fixture)
@pytest.fixture
def client():
    # Configure the app for testing
    app.config['TESTING'] = True
    app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///:memory:' # In-memory RAM database
    app.config['WTF_CSRF_ENABLED'] = False # Disable CSRF protection to make testing easier

    with app.test_client() as client:
        with app.app_context():
            db.create_all() # Create tables
        yield client
        with app.app_context():
            db.drop_all()   # Drop tables after the test finishes

# ---------------------------------------------------------
# Test 1: US 5.8 (Success Case)
# ---------------------------------------------------------
def test_create_event_success(client):
    """
    US 5.8: Verify creating an event with valid data (Future Date).
    Expected: Success (Redirect 302) + Saved in DB.
    """
    # Simulate Admin Login
    with client.session_transaction() as sess:
        sess['user_id'] = "admin_user"
        sess['role'] = "admin"

    # Prepare Valid Data (Future Date)
    tomorrow = date.today() + timedelta(days=1)
    valid_data = {
        "name": "Future Event",
        "date": tomorrow.strftime("%Y-%m-%d"),
        "tracks": "AI",
        "description": "Valid event"
    }

    # Send Request
    response = client.post('/api/events', data=valid_data)

    # Assertions
    assert response.status_code == 302
    with app.app_context():
        event = Event.query.filter_by(name="Future Event").first()
        assert event is not None
        print("\n✅ [US 5.8] Success case passed.")

# ---------------------------------------------------------
# Test 2: US 5.9 (Failure Case - Past Date) - NEW!
# ---------------------------------------------------------
def test_create_event_past_date_failure(client):
    """
    US 5.9: Verify submitting a past date returns a validation error.
    Expected: Redirect (302) but NOT saved in DB.
    """
    
    # 1. Simulate Admin Login
    with client.session_transaction() as sess:
        sess['user_id'] = "admin_user"
        sess['role'] = "admin"

    # 2. Prepare Invalid Data (Yesterday's Date)
    yesterday = date.today() - timedelta(days=1)
    invalid_data = {
        "name": "Past Event Failure Test",
        "date": yesterday.strftime("%Y-%m-%d"), # Past date
        "tracks": "Cyber",
        "description": "This should fail"
    }

    # 3. Send the Request
    response = client.post('/api/events', data=invalid_data)

    # 4. Assertions (Validation)
    
    # Must return a Redirect (302) because it uses Flash messages
    assert response.status_code == 302 

    # Most Important: Verify that the event was NOT saved to the database
    with app.app_context():
        event = Event.query.filter_by(name="Past Event Failure Test").first()
        assert event is None # Result must be empty (None)
        print("\n✅ [US 5.9] Past date rejection passed (Event not saved).")