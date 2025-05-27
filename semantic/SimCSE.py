import os
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import numpy as np
from sentence_transformers import SentenceTransformer, util
import psycopg2
from psycopg2.extras import DictCursor
from concurrent.futures import ThreadPoolExecutor
from typing import List

app = FastAPI()

DATABASE_URL = os.getenv("DATABASE_URL",
                         "postgresql://fuc_user:DlTsF62MKiIjXQ0IGStodubjDTqQ1hRV@dpg-d0qkfe3e5dus739l19dg-a.oregon-postgres.render.com/fuc")

model = SentenceTransformer("sentence-transformers/stsb-roberta-large")
executor = ThreadPoolExecutor(max_workers=4)  # Singleton Executor

class SemanticRequest(BaseModel):
    topic_id: str
    semester_ids: List[str]
    capstone_id: str
    campus_id: str


def get_topics(semester_ids: List[str], capstone_id: str, campus_id: str, is_current_semester: bool, exclude_topic_id: str = None):
    """Retrieve topics based on the semester type, optionally excluding a specific topic."""
    with psycopg2.connect(DATABASE_URL) as conn:
        with conn.cursor(cursor_factory=DictCursor) as cursor:
            if is_current_semester:
                # Exclude "Failed" topics and the topic itself
                cursor.execute('''
                    SELECT "Id", "Description", "EnglishName"
                    FROM "Topic"
                    WHERE "SemesterId" = ANY(%s) 
                      AND "Status" != %s
                      AND "CapstoneId" = %s
                      AND "CampusId" = %s
                      AND "Id" != %s;
                ''', (semester_ids, 'Rejected', capstone_id, campus_id, exclude_topic_id))
            else:
                # Only include "Passed" topics for past semesters
                cursor.execute('''
                    SELECT "Id", "Description", "EnglishName"
                    FROM "Topic"
                    WHERE "SemesterId" = ANY(%s) 
                      AND "Status" = %s
                      AND "CapstoneId" = %s
                      AND "CampusId" = %s;
                ''', (semester_ids, 'Approved', capstone_id, campus_id))

            topics = [
                {
                    "id": str(row["Id"]),
                    "context": row["Description"],
                    "english_name": row["EnglishName"]
                }
                for row in cursor.fetchall()
            ]

    return topics

def get_topic_by_id(topic_id: str):
    """Retrieve a single topic by ID."""
    with psycopg2.connect(DATABASE_URL) as conn:
        with conn.cursor(cursor_factory=DictCursor) as cursor:
            cursor.execute('''
                SELECT "Id", "Description", "EnglishName"
                FROM "Topic"
                WHERE "Id" = %s;
            ''', (topic_id,))
            row = cursor.fetchone()

    return {
        "id": str(row["Id"]),
        "context": row["Description"],
        "english_name": row["EnglishName"]
    } if row else None

def compute_embedding(text):
    return model.encode(text)

def find_best_match(topic_id: str, semester_ids: List[str], capstone_id: str, campus_id: str, is_current_semester: bool):
    topics = get_topics(semester_ids, capstone_id, campus_id, is_current_semester, exclude_topic_id=topic_id if is_current_semester else None)
    new_topic = get_topic_by_id(topic_id)

    if not new_topic:
        raise HTTPException(status_code=404, detail="Topic not found or does not match required status.")

    if not topics:
        return {}

    new_embedding = model.encode(new_topic["context"])

    # Tận dụng executor ở cấp module
    topic_texts = [t["context"] for t in topics]
    topic_embeddings = list(executor.map(compute_embedding, topic_texts))

    similarities = util.cos_sim(new_embedding, np.array(topic_embeddings)).numpy().flatten()

    matching_topics = {
        topics[i]["id"]: {
            "similarity": round(float(sim) * 100, 2),
            "english_name": topics[i]["english_name"]
        }
        for i, sim in enumerate(similarities) if sim >= 0.6
    }

    return matching_topics

@app.get("/test")
def get_test():
    return {"message": "Service is running on port 9000"}

@app.post("/previous/semantic")
def get_past_semesters_match(request: SemanticRequest):
    """Find matching topics for a topic across multiple past semesters."""
    try:
        return find_best_match(request.topic_id, request.semester_ids, request.capstone_id, request.campus_id,
                               is_current_semester=False)
    except Exception as e:
        print(f"Error in /semantic/: {e}")
    raise HTTPException(status_code=500, detail=str(e))

@app.get("/semantic/{campus_id}/{capstone_id}/{semester_id}/{topic_id}")
def get_current_semester_match(campus_id: str, capstone_id: str, semester_id: str, topic_id: str):
    """Find matching topics from the current semester (excluding 'Fail' topics and itself)."""
    try:
        return find_best_match(topic_id, [semester_id], capstone_id, campus_id, is_current_semester=True)
    except Exception as e:
        print(f"Error in /semantic/: {e}")
        raise HTTPException(status_code=500, detail=str(e))

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=9000)