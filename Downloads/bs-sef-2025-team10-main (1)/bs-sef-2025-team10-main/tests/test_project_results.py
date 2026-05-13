import sys
import os
import unittest

# Add parent directory to path to find app.py
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

from app import app, db, User, Project

class TestProjectResults(unittest.TestCase):
    def setUp(self):
        app.config['TESTING'] = True
        app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///:memory:'
        app.config['WTF_CSRF_ENABLED'] = False
        self.app = app.test_client()
        
        with app.app_context():
            db.create_all()
            # 1. Create Student A and their project
            student_a = User(full_name="Student A", email="a@test.com", password_hash="hash", role="student")
            db.session.add(student_a)
            
            # 2. Create Student B (The "Attacker" or "Unauthorized" user)
            student_b = User(full_name="Student B", email="b@test.com", password_hash="hash", role="student")
            db.session.add(student_b)
            
            # 3. Create Project for Student A
            proj_a = Project(
                name="Student A Project", category="AI", description="Desc",
                team_members="A", contact_email="a@test.com", student_id=1,
                average_score=9.0, feedback="Great!", is_published=False
            )
            db.session.add(proj_a)
            db.session.commit()

    def test_unpublished_case(self):
        """Test 11.4: Ensure scores are hidden when is_published is False"""
        with self.app.session_transaction() as sess:
            sess['user_id'] = 1 # Logged in as Student A
        
        response = self.app.get('/my-results')
        self.assertIn(b"Results Pending", response.data)
        self.assertNotIn(b"9.0", response.data)

    def test_published_case(self):
        """Test 11.4: Ensure score and feedback appear when published"""
        with app.app_context():
            proj = db.session.get(Project, 1)
            proj.is_published = True
            db.session.commit()
            
        with self.app.session_transaction() as sess:
            sess['user_id'] = 1 # Logged in as Student A
            
        response = self.app.get('/my-results')
        self.assertIn(b"9.0", response.data)
        self.assertIn(b"Great!", response.data)

    def test_unauthorized_access_prevention(self):
        """Test 11.4: Ensure Student B cannot see Student A's results"""
        with self.app.session_transaction() as sess:
            sess['user_id'] = 2 # Logged in as Student B
            sess['role'] = 'student' # Ensure role is set too
            
        response = self.app.get('/my-results', follow_redirects=True)
        
        # Update this line to match the text in your alert div:
        self.assertIn(b"No project found for your account.", response.data)
        
        # Also verify Student A's project name isn't visible to Student B
        self.assertNotIn(b"Student A Project", response.data)

    def tearDown(self):
        with app.app_context():
            db.session.remove()
            db.drop_all()

if __name__ == "__main__":
    unittest.main()