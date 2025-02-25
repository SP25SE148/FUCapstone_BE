import os
from fastapi import FastAPI, HTTPException
import numpy as np
from sentence_transformers import SentenceTransformer, util
import psycopg2
from psycopg2.extras import DictCursor
from concurrent.futures import ThreadPoolExecutor

app = FastAPI()

DATABASE_URL = os.getenv("DATABASE_URL", "postgresql://postgres:postgrespw@postgres:5432/fuc")

model = SentenceTransformer("sentence-transformers/stsb-roberta-large")
executor = ThreadPoolExecutor(max_workers=4)

def get_pass_topics():
    conn = psycopg2.connect(DATABASE_URL)
    cursor = conn.cursor(cursor_factory=DictCursor)  # Use DictCursor for key-value access

    cursor.execute('SELECT "Id", "Description", "EnglishName" FROM "Topic" WHERE "Status" = %s;', ('Pass',))

    topics = [{"id": str(row["Id"]), "context": row["Description"], "english_name": row["EnglishName"]} for row in cursor.fetchall()]

    cursor.close()
    conn.close()
    return topics

def compute_embedding(text):
    return model.encode(text)

@app.get("/test")
def get_test():
    return {"message": "Service is running on port 9000"}

@app.get("/semantic/{topic_id}")  # Keep endpoint but remove topic_id from response
def find_best_match(topic_id: str):
    topics = get_pass_topics()
    new_topic = next((t for t in topics if t["id"] == topic_id), None)
    
    if not new_topic:
        raise HTTPException(status_code=404, detail="Topic not found or not 'Pass'.")

    new_embedding = model.encode(new_topic["context"])
    
    with executor:
        topic_embeddings = list(executor.map(compute_embedding, [t["context"] for t in topics]))

    similarities = util.cos_sim(new_embedding, np.array(topic_embeddings)).numpy().flatten()

    matching_topics = {
        topics[i]["id"]: {
            "similarity": round(float(sim) * 100, 2),
            "english_name": topics[i]["english_name"]
        }
        for i, sim in enumerate(similarities) if sim >= 0.6
    }

    return matching_topics  # Remove topic_id from response

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=9000)  # Ensure port 9000 is used inside the container
