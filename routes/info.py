import flask
from app import app, language, sprites, moveTypes
from utils.helpers import get_pokemon_from_flask_request
from utils.pkmn import Pokemon

@app.route('/api/v2/info', methods=['POST'])
def info():
    pkmn, error = get_pokemon_from_flask_request(flask.request)
    
    if error is not None:
        return flask.jsonify(error), 400
    
    return flask.make_response(Pokemon(pkmn, language, moveTypes, sprites).toJSON(), 200, {"Content-Type": "application/json"})