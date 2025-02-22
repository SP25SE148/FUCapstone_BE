FROM python:3.9

WORKDIR /app

# Install necessary dependencies
RUN pip install --no-cache-dir fastapi uvicorn numpy sentence-transformers psycopg2

# Expose port for FastAPI
EXPOSE 9000

# Copy the FastAPI code into the container from the semantic folder
COPY semantic/SimCSE.py .

# Command to run the FastAPI app
CMD ["uvicorn", "SimCSE:app", "--host", "0.0.0.0", "--port", "9000"]
