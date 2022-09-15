import flask
from app import app, language, sprites, moveTypes
from utils.helpers import get_pkmn
from utils.pkmn import Pokemon

@app.route('/api/v2/info', methods=['POST'])
def info():
    # We're expecting the following to be passed in the body
    # pkmn: this is a file, the pokemon file, no more than 400 bytes for now
    # generation: this is a string, the generation of the pokemon
    
    # let's try to get the pokemon file
    try:
        pkmn = flask.request.files['pkmn']
    except:
        return flask.jsonify({"error": "Missing pokemon file"}), 400

    # let's try to get the generation
    try:
        generation = flask.request.form['generation']
    except:
        generation = None

    # copy the file to a byte array
    data = bytearray(pkmn.read())
    pkmn.close()
    pkmn = get_pkmn(data, generation)
    if pkmn is None:
        return flask.jsonify({"error": "Invalid pokemon file"}), 400
    

    return flask.make_response(Pokemon(pkmn, language, moveTypes, sprites).toJSON(), 200, {"Content-Type": "application/json"})