from app import app
from routes import info, legality

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=4005, debug=True, load_dotenv=True)
