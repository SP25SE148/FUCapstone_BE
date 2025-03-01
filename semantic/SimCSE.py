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

DATABASE_URL = os.getenv("DATABASE_URL", "postgresql://postgres:postgrespw@postgres:5432/fuc")

model = SentenceTransformer("sentence-transformers/stsb-roberta-large")
executor = ThreadPoolExecutor(max_workers=4)  # Singleton Executor

class SemanticRequest(BaseModel):
    topic_id: str
    semester_ids: List[str]

def get_topics(semester_ids: List[str], is_current_semester: bool):
    """Retrieve topics based on the semester type."""
    with psycopg2.connect(DATABASE_URL) as conn:
        with conn.cursor(cursor_factory=DictCursor) as cursor:
            if is_current_semester:
                cursor.execute('''
                    SELECT "Id", "Description", "EnglishName"
                    FROM "Topic"
                    WHERE "SemesterId" = ANY(%s) AND "Status" != %s;
                ''', (semester_ids, 'Failed'))
            else:
                cursor.execute('''
                    SELECT "Id", "Description", "EnglishName"
                    FROM "Topic"
                    WHERE "SemesterId" = ANY(%s) AND "Status" = %s;
                ''', (semester_ids, 'Passed'))

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

def find_best_match(topic_id: str, semester_ids: List[str], is_current_semester: bool):
    topics = get_topics(semester_ids, is_current_semester)
    new_topic = next((t for t in topics if t["id"] == topic_id), None) if is_current_semester else get_topic_by_id(topic_id)

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

@app.post("/semantic")
def get_past_semesters_match(request: SemanticRequest):
    """Find matching topics for a topic across multiple past semesters."""
    return find_best_match(request.topic_id, request.semester_ids, is_current_semester=False)


@app.get("/semantic/{semester_id}/{topic_id}")
def get_current_semester_match(semester_id: str, topic_id: str):
    """Find matching topics from the current semester (excluding 'Fail' topics and itself)."""
    return find_best_match(topic_id, [semester_id], is_current_semester=True)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=9000)

# import os
# from fastapi import FastAPI, HTTPException, Query
# from pydantic import BaseModel
# import numpy as np
# from sentence_transformers import SentenceTransformer, util
# import psycopg2
# from psycopg2.extras import DictCursor
# from concurrent.futures import ThreadPoolExecutor
# from typing import List
# 
# app = FastAPI()
# 
# DATABASE_URL = os.getenv("DATABASE_URL", "postgresql://postgres:postgrespw@postgres:5432/fuc")
# 
# model = SentenceTransformer("sentence-transformers/stsb-roberta-large")
# executor = ThreadPoolExecutor(max_workers=4)
# 
# class SemanticRequest(BaseModel):
#     topic_id: str
#     semester_ids: List[str]
# 
# def get_topics(semester_ids: List[str], is_current_semester: bool):
#     """Retrieve topics based on the semester type."""
#     conn = psycopg2.connect(DATABASE_URL)
#     cursor = conn.cursor(cursor_factory=DictCursor)
# 
#     if is_current_semester:
#         # For the current semester, exclude "Fail" topics and exclude the topic itself
#         cursor.execute('''
#             SELECT "Id", "Description", "EnglishName"
#             FROM "Topic"
#             WHERE "SemesterId" = ANY(%s) AND "Status" != %s;
#         ''', (semester_ids, 'Failed'))
#     else:
#         # For previous semesters, only include "Pass" topics
#         cursor.execute('''
#             SELECT "Id", "Description", "EnglishName"
#             FROM "Topic"
#             WHERE "SemesterId" = ANY(%s) AND "Status" = %s;
#         ''', (semester_ids, 'Passed'))
# 
#     topics = [
#         {
#             "id": str(row["Id"]),
#             "context": row["Description"],
#             "english_name": row["EnglishName"]
#         }
#         for row in cursor.fetchall()
#     ]
# 
#     cursor.close()
#     conn.close()
#     return topics
# 
# def get_topic_by_id(topic_id: str):
#     """Retrieve a single topic by ID."""
#     conn = psycopg2.connect(DATABASE_URL)
#     cursor = conn.cursor(cursor_factory=DictCursor)
# 
#     cursor.execute('''
#         SELECT "Id", "Description", "EnglishName"
#         FROM "Topic"
#         WHERE "Id" = %s;
#     ''', (topic_id,))
# 
#     row = cursor.fetchone()
#     cursor.close()
#     conn.close()
# 
#     if row:
#         return {
#             "id": str(row["Id"]),
#             "context": row["Description"],
#             "english_name": row["EnglishName"]
#         }
#     return None
# 
# def compute_embedding(text):
#     return model.encode(text)
# 
# def find_best_match(topic_id: str, semester_ids: List[str], is_current_semester: bool):
#     topics = get_topics(semester_ids, is_current_semester)
#     new_topic = next((t for t in topics if t["id"] == topic_id), None) if is_current_semester else get_topic_by_id(topic_id)
# 
#     if not new_topic:
#         raise HTTPException(status_code=404, detail="Topic not found or does not match required status.")
# 
#     if not topics:
#         return {}
# 
#     new_embedding = model.encode(new_topic["context"])
#     
#     with executor:
#         topic_embeddings = list(executor.map(compute_embedding, [t["context"] for t in topics]))
# 
#     similarities = util.cos_sim(new_embedding, np.array(topic_embeddings)).numpy().flatten()
# 
#     matching_topics = {
#         topics[i]["id"]: {
#             "similarity": round(float(sim) * 100, 2),
#             "english_name": topics[i]["english_name"]
#         }
#         for i, sim in enumerate(similarities) if sim >= 0.6
#     }
# 
#     return matching_topics  # Removed topic_id from response
# 
# @app.get("/test")
# def get_test():
#     return {"message": "Service is running on port 9000"}
# 
# @app.post("/semantic")
# def get_past_semesters_match(request: SemanticRequest):
#     """Find matching topics for a topic across multiple past semesters."""
#     return find_best_match(request.topic_id, request.semester_ids, is_current_semester=False)
# 
# @app.get("/semantic/{semester_id}/{topic_id}")  
# def get_current_semester_match(semester_id: str, topic_id: str):
#     """Find matching topics from the current semester (excluding 'Fail' topics and itself)."""
#     return find_best_match(topic_id, [semester_id], is_current_semester=True)
# 
# if __name__ == "__main__":
#     import uvicorn
#     uvicorn.run(app, host="0.0.0.0", port=9000)
