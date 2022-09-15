import base64
import flask
from app import app
from utils.helpers import get_pokemon_from_flask_request
from utils.legality import legality_check, legalize
import traceback

@app.route('/api/v2/legality', methods=['POST'])
def check_legality():
    pkmn, error = get_pokemon_from_flask_request(flask.request)
    
    if error is not None:
        return flask.jsonify(error), 400
    valid, report = legality_check(pkmn)

    return flask.make_response({
        "legal": valid,
        "report": report.split("\n")
    }, 200, {"Content-Type": "application/json"})
    

@app.route('/api/v2/legalize', methods=['POST'])
def auto_legality():
    pkmn, error = get_pokemon_from_flask_request(flask.request)
    
    if error is not None:
        return flask.jsonify(error), 400

    try:
        valid, report = legality_check(pkmn)
        if valid:
            return flask.make_response({"error": "this pokemon is already legal!"}, 400, {"Content-Type": "application/json"})
    except:
        traceback.print_exc()
        return flask.make_response({"error": "error while checking legality"}, 400, {"Content-Type": "application/json"})
    
    try:
        legalized = legalize(pkmn)
    except Exception:
        print(traceback.format_exc())
        return flask.jsonify({"error": "something went wrong with legalizing your Pokemon"}), 500

    if legalized is None:
        return flask.make_response({
            "legal": False,
            "pokemon": None,
            "report": report.split("\n")
        }, 200, {"Content-Type": "application/json"})

    return flask.make_response({
        "legal": True,
        "pokemon": base64.b64encode(bytearray(byte for byte in pkmn.DecryptedBoxData)).decode('UTF-8'),
        "report": report.split("\n")
    }, 200, {"Content-Type": "application/json"})
    