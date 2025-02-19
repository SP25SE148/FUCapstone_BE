import os
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import numpy as np
from sentence_transformers import SentenceTransformer, util
import psycopg2
from concurrent.futures import ThreadPoolExecutor

app = FastAPI()

DATABASE_URL = os.getenv("DATABASE_URL", "postgresql://postgres:postgrespw@postgres:5432/fuc")

model = SentenceTransformer("sentence-transformers/stsb-roberta-large")
executor = ThreadPoolExecutor(max_workers=4)

class ContextRequest(BaseModel):
    topic_id: int

def get_pass_topics():
    conn = psycopg2.connect(DATABASE_URL)
    cursor = conn.cursor()
    cursor.execute("SELECT id, context FROM topics WHERE status = 'Pass';")
    topics = [{"id": row[0], "context": row[1]} for row in cursor.fetchall()]
    cursor.close()
    conn.close()
    return topics

def compute_embedding(text):
    return model.encode(text)

@app.get("/test")
def get_test():
    return {"message": "Ok"}

@app.post("/sematic")
def find_best_match(request: ContextRequest):
    topics = get_pass_topics()
    new_topic = next((t for t in topics if t["id"] == request.topic_id), None)
    if not new_topic:
        raise HTTPException(status_code=404, detail="Topic not found or not 'Pass'.")

    new_embedding = model.encode(new_topic["context"])
    with executor:
        topic_embeddings = list(executor.map(compute_embedding, [t["context"] for t in topics]))

    similarities = util.cos_sim(new_embedding, np.array(topic_embeddings)).numpy().flatten()
    matching_topics = {topics[i]["id"]: round(float(sim) * 100, 2) for i, sim in enumerate(similarities) if sim >= 0.6}

    return {"topic_id": request.topic_id, "matching_topics": matching_topics}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="127.0.0.1", port=9000)
