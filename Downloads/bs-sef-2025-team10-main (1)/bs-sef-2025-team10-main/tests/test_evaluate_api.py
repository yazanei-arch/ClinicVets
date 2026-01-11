def test_api_evaluate_rejects_score_out_of_range(client):
    payload = {
        "project_id": 1,
        "innovation": 15,   # invalid
        "clarity": 5,
        "completeness": 5,
        "comments": "test"
    }

    resp = client.post("/api/evaluate", json=payload)

    assert resp.status_code == 400
    data = resp.get_json()
    assert data["ok"] is False
    assert "Scores" in data.get("error", "")
