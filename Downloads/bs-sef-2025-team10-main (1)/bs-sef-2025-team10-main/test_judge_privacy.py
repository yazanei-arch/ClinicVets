import unittest
import os
from datetime import datetime
from app import app, db, User, Project, Event

class TestJudgeProjectIsolation(unittest.TestCase):
    
    def setUp(self):
        # 1. إعدادات الاختبار
        app.config['TESTING'] = True
        app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///test_temp.db'
        app.config['WTF_CSRF_ENABLED'] = False
        
        self.client = app.test_client()
        
        # 2. إنشاء البيانات وحفظ الـ IDs
        with app.app_context():
            db.create_all()
            
            # Create Event
            event = Event(name="Test Event", date=datetime(2025, 1, 1))
            db.session.add(event)
            db.session.commit() # Commit to get ID
            event_id = event.id

            # Create Student
            student = User(full_name="Student", email="std@test.com", password_hash="x", role="student")
            db.session.add(student)
            db.session.commit()
            student_id = student.id

            # Create Judge A
            judge_a = User(full_name="Judge A", email="judgeA@test.com", password_hash="hash", role="judge")
            db.session.add(judge_a)
            db.session.commit()
            # فكرة الحل: نحفظ الـ ID فوراً في متغير منفصل
            self.judge_a_id = judge_a.id 
            
            # Create Judge B
            judge_b = User(full_name="Judge B", email="judgeB@test.com", password_hash="hash", role="judge")
            db.session.add(judge_b)
            db.session.commit()
            self.judge_b_id = judge_b.id

            # Create Projects using the IDs
            p1 = Project(name="Project A", description="Desc", category="AI", team_members="Team A", contact_email="a@a.com", 
                         event_id=event_id, student_id=student_id, judge_id=self.judge_a_id)
            
            p2 = Project(name="Project B", description="Desc", category="Web", team_members="Team B", contact_email="b@b.com", 
                         event_id=event_id, student_id=student_id, judge_id=self.judge_b_id)
            
            db.session.add_all([p1, p2])
            db.session.commit()

    def tearDown(self):
        with app.app_context():
            db.session.remove()
            db.drop_all()
        
        if os.path.exists("test_temp.db"):
            os.remove("test_temp.db")

    # --- THE MAIN TEST (US 6.6) ---
    def test_judge_cannot_see_others_projects(self):
        # Step 1: Force Login (Session Bypass) using the stored ID
        with self.client.session_transaction() as sess:
            # هنا نستخدم الـ ID المحفوظ (رقم صحيح) بدلاً من الكائن
            sess['user_id'] = self.judge_a_id
            sess['role'] = 'judge'
            sess['_fresh'] = True

        # Step 2: Request the API
        response = self.client.get('/api/my-projects')
        
        self.assertEqual(response.status_code, 200)
        data = response.get_json()

        # Step 3: Verify Results
        print(f"\nFound {len(data)} projects for Judge A.")
        
        # Judge A should see exactly 1 project
        self.assertEqual(len(data), 1, "Judge A should see exactly 1 project")
        self.assertEqual(data[0]['project_name'], "Project A")
        
        # Verify Judge B's project is NOT there
        project_names = [p['project_name'] for p in data]
        self.assertNotIn("Project B", project_names, "Security Fail: Judge A saw Judge B's project!")

        print("✅ Success: Judge A sees only their assigned projects.")

if __name__ == "__main__":
    unittest.main()