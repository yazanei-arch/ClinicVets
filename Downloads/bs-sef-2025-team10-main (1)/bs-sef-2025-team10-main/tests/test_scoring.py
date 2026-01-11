import sys
import os
import unittest

# Add the parent directory to the path so we can find app/scoring_service
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

from scoring_service import calculate_weighted_score

class TestScoring(unittest.TestCase):
    def test_multi_judge_average(self):
        # Mock scores for 3 judges (Innovation, Clarity, Completeness)
        eval_1 = {"innovation": 10, "clarity": 10, "completeness": 10} # 10.0
        eval_2 = {"innovation": 8, "clarity": 8, "completeness": 8}    # 8.0
        eval_3 = {"innovation": 9, "clarity": 9, "completeness": 9}    # 9.0

        # Calculate individual scores
        score_1 = calculate_weighted_score(eval_1)
        score_2 = calculate_weighted_score(eval_2)
        score_3 = calculate_weighted_score(eval_3)

        # Simulate the cumulative average logic
        avg_score = score_1  # 10.0
        avg_score = (avg_score + score_2) / 2 # 9.0
        avg_score = (avg_score + score_3) / 2 # 9.0

        self.assertEqual(avg_score, 9.0, f"Expected 9.0 but got {avg_score}")

if __name__ == "__main__":
    unittest.main()