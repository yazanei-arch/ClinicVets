# tests/test_auto_assign.py

from scoring_service import auto_assign_projects_to_judges


def test_cyber_tag_matches():
    judges = [
        {"id": 1, "name": "Judge A", "tags": ["cyber"], "max_projects": 3},
    ]

    projects = [
        {"id": 101, "title": "Project Cyber", "tags": ["cyber"]},
    ]

    assignments, loads = auto_assign_projects_to_judges(judges, projects)

    assert assignments[101] == 1
    assert loads[1] == 1

from scoring_service import auto_assign_projects_to_judges


def test_judge_with_zero_matching_tags_not_assigned():
    judges = [
        {"id": 1, "name": "Judge A", "tags": ["AI"], "max_projects": 3},
    ]
    projects = [
        {"id": 201, "title": "Cyber Project", "tags": ["Cyber"]},
    ]

    assignments, loads = auto_assign_projects_to_judges(judges, projects)

    assert assignments[201] is None
    assert loads[1] == 0
