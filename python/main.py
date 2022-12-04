import base64
import signal
from utils.load import *
import argparse
from utils.helpers import get_pokemon_from_base64, LanguageStrings, MoveTypes
from utils.legality import legality_check, legalize, cancel
from utils.pkmn import Pokemon
from utils.sprites import Sprites
import json
import sys

language = LanguageStrings()
sprites = Sprites()
moveTypes = MoveTypes()

def handleArgs():
    parser = argparse.ArgumentParser()
    parser.add_argument('--mode', type=str, required=True, choices=['legalize', 'report', 'info'], help='The function to perform')
    parser.add_argument('--pkmn', type=str, required=True, help='The base64 encoded pokemon to perform the function on')
    parser.add_argument('--generation', type=str, required=False, help='The generation of the pokemon to perform the function on')
    return parser.parse_args()

def checkLegality(pkmn):
    valid, report = legality_check(pkmn)
    sys.stdout.write(json.dumps({
        "legal": valid,
        "report": report.split("\n")
    }, ensure_ascii=False))
    sys.stdout.write("\n")
    sys.stdout.flush()

def autoLegality(pkmn, generation):
    valid, report = legality_check(pkmn)
    if valid:
        sys.stdout.write(json.dumps({"error": "this pokemon is already legal!"}))
        sys.stdout.write("\n")
        sys.stdout.flush()
        return
    
    legalized = legalize(pkmn, generation)
    if legalized is None:
        sys.stdout.write(json.dumps({
            "legal": False,
            "pokemon": None,
            "report": report.split("\n")
        }, ensure_ascii=False))
        sys.stdout.write("\n")
        sys.stdout.flush()
        return
    
    sys.stdout.write(json.dumps({
        "legal": True,
        "pokemon": base64.b64encode(bytearray(byte for byte in legalized.DecryptedBoxData)).decode('UTF-8'),
        "report": report.split("\n")
    }))
    sys.stdout.write("\n")
    sys.stdout.flush()


def getInfo(pkmn):
        sys.stdout.write(Pokemon(pkmn, language, moveTypes, sprites).toJSON())
        sys.stdout.write("\n")
        sys.stdout.flush()

def handleTermination(signum, frame):
    cancel()
    sys.exit(0)

if __name__ == '__main__':
    args = handleArgs()

    signal.signal(signal.SIGINT, handleTermination)
    signal.signal(signal.SIGTERM, handleTermination)

    pkmn, error = get_pokemon_from_base64(args.pkmn, args.generation)
    if error is not None:
        sys.stderr.write(json.dumps(error, ensure_ascii=False))
        sys.stderr.write("\n")
        sys.stderr.flush()
        sys.exit(1)
    
    if args.mode == 'report':
        result = checkLegality(pkmn)
    elif args.mode == 'legalize':
        try:
            result = autoLegality(pkmn, args.generation)
        except Exception as e:
            sys.stderr.write(json.dumps({"error": "something went wrong with legalizing your Pokemon"}))
            sys.stderr.write("\n")
            sys.stderr.flush()
            print(e)
            sys.exit(1)
    elif args.mode == 'info':
        try:
            getInfo(pkmn)
        except Exception as e:
            sys.stderr.write(json.dumps({"error": "something went wrong with getting info for your Pokemon"}))
            sys.stderr.write("\n")
            sys.stderr.flush()
            print(e)
            sys.exit(1)
