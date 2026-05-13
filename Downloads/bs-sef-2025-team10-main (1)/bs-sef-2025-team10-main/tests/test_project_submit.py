import sys
import os

# This adds the parent directory to the Python path
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

# Now the existing imports will work
from app import app, db, User, Project
import unittest

import unittest
from app import app, db, User, Project

class TestProjectSubmission(unittest.TestCase):
    def setUp(self):
        # Setup temporary environment
        app.config['TESTING'] = True
        app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///:memory:' 
        app.config['WTF_CSRF_ENABLED'] = False # Disables CSRF for easier testing
        self.app = app.test_client()
        
        with app.app_context():
            db.create_all()
            # Create a mock student to "log in"
            student = User(
                full_name="Alice Student", 
                email="alice@test.com", 
                password_hash="fakehash", 
                role="student"
            )
            db.session.add(student)
            db.session.commit()

    def login_mock_student(self):
        """Simulates the session login we added to app.py"""
        with self.app.session_transaction() as sess:
            sess['user_id'] = 1
            sess['role'] = 'student'

    def test_successful_submission_and_linking(self):
        """Verify project is saved and linked to the student_id (US 10.3)"""
        self.login_mock_student()
        
        response = self.app.post('/submit-project', data={
            'name': 'Green Energy App',
            'category': 'Sustainability',
            'description': 'An app to track carbon footprints.',
            'team_members': 'Alice, Charlie',
            'contact_email': 'alice@test.com'
        }, follow_redirects=True)

        # Check for the success message from Subtask 10.2
        self.assertIn(b"was successfully submitted", response.data)
        # Verify DB entry exists and has the correct student_id
        with app.app_context():
            proj = Project.query.filter_by(name='Green Energy App').first()
            self.assertIsNotNone(proj)
            self.assertEqual(proj.student_id, 1)

    def test_failed_submission_missing_fields(self):
        """Verify error message when fields are empty (US 10.2)"""
        self.login_mock_student()
        response = self.app.post('/submit-project', data={
            'name': '', # Missing name should trigger failure
            'category': 'Sustainability'
        }, follow_redirects=True)
        
        self.assertIn(b"failed", response.data)

    def tearDown(self):
        with app.app_context():
            db.session.remove()
            db.drop_all()

if __name__ == "__main__":
    unittest.main()