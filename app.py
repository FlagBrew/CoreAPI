import flask
from utils.load import *
from utils.helpers import LanguageStrings, MoveTypes
from utils.sprites import Sprites

app = flask.Flask(__name__)
# Should be no more than 1 kb
app.config['MAX_CONTENT_LENGTH'] = 1024

language = LanguageStrings()
sprites = Sprites()
moveTypes = MoveTypes()
