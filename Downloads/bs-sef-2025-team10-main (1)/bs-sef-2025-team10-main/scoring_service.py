# scoring_service.py

def calculate_weighted_score(scores, track=None):
    """
    US 9.2: Calculate Final Score based on specific weights.
    Formula: (Innovation * 0.4) + (Clarity * 0.3) + (Completeness * 0.3)
    """
    # Define weights
    weights = {
        "innovation": 0.4,
        "clarity": 0.3,
        "completeness": 0.3
    }
    
    # Calculate weighted sum
    # .get(key, 0) ensures the code doesn't crash if a score is missing
    final_score = (
        (scores.get("innovation", 0) * weights["innovation"]) +
        (scores.get("clarity", 0) * weights["clarity"]) +
        (scores.get("completeness", 0) * weights["completeness"])
    )
    
    # Round to 2 decimal places for a clean UI
    return round(float(final_score), 2)

def get_judge_projects_mock(judge_id):
    """Mock data for US-10 dashboard."""
    return [
        {"id": 101, "name": "Eco-Tracker", "track": "IOT", "status": "Pending"},
        {"id": 102, "name": "Health-Bot", "track": "AI", "status": "Completed"}
    ]
def normalize_tags(tags_value) -> list[str]:
    if not tags_value:
        return []
    if isinstance(tags_value, list):
        return [t.strip().lower() for t in tags_value if str(t).strip()]
    return [t.strip().lower() for t in str(tags_value).split(",") if t.strip()]

def fetch_judges_and_projects(db):
    judges_rows = db.execute("SELECT id, name, tags, max_projects FROM judges").fetchall()
    projects_rows = db.execute("SELECT id, title, tags FROM projects").fetchall()

    judges = [
        {
            "id": r["id"],
            "name": r["name"],
            "tags": normalize_tags(r["tags"]),
            "max_projects": r["max_projects"],
        }
        for r in judges_rows
    ]

    projects = [
        {
            "id": r["id"],
            "title": r["title"],
            "tags": normalize_tags(r["tags"]),
        }
        for r in projects_rows
    ]

    return judges, projects

def auto_assign_projects_to_judges(judges, projects, default_max_projects=3):
    loads = {j["id"]: 0 for j in judges}
    max_allowed = {j["id"]: (j.get("max_projects") or default_max_projects) for j in judges}
    judge_tags = {j["id"]: set(t.lower() for t in j.get("tags", [])) for j in judges}

    assignments = {}

    for p in projects:
        pid = p["id"]
        ptags = set(t.lower() for t in p.get("tags", []))

        candidates = []
        for j in judges:
            jid = j["id"]
            if loads[jid] >= max_allowed[jid]:
                continue
            score = len(ptags & judge_tags[jid])
            candidates.append((score, loads[jid], jid))

        if not candidates:
            assignments[pid] = None
            continue

        candidates.sort(key=lambda x: (-x[0], x[1], x[2]))
        best_score, _, best_jid = candidates[0]

        # ✅ US4.9 rule: if no matching tags, do NOT assign
        if best_score == 0:
            assignments[pid] = None
            continue

        assignments[pid] = best_jid
        loads[best_jid] += 1

    return assignments, loads

def save_judge_assignments(db, assignments: dict[int, int | None], clear_existing: bool = True) -> int:
    """
    Save assignment pairs into Judge_Assignments.

    assignments: dict[project_id] = judge_id (or None)
    clear_existing: if True, remove old assignments before inserting new ones.

    Returns: number of inserted rows
    """
    # Optionally clear old rows
    if clear_existing:
        db.execute("DELETE FROM Judge_Assignments")

    rows = []
    for project_id, judge_id in assignments.items():
        if judge_id is None:
            continue
        rows.append((judge_id, project_id))

    if not rows:
        db.commit()
        return 0

    db.executemany(
        "INSERT OR REPLACE INTO Judge_Assignments (judge_id, project_id) VALUES (?, ?)",
        rows
    )
    db.commit()
    return len(rows)
def fetch_current_assignments(db) -> dict[int, int]:
    """
    Returns: dict[project_id] = judge_id
    """
    rows = db.execute(
        "SELECT project_id, judge_id FROM Judge_Assignments"
    ).fetchall()

    return {r["project_id"]: r["judge_id"] for r in rows}
