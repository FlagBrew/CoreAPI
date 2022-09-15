import base64
import json
import pytest
from utils.load import *
from utils.helpers import get_pkmn, LanguageStrings, MoveTypes
from utils.pkmn import Pokemon
from utils.sprites import Sprites

class TestPokemonSummary:
    strings = None
    moveTypes = None
    sprites = None

    def setup(self):
        self.strings = LanguageStrings()
        self.moveTypes = MoveTypes()
        self.sprites = Sprites()

    @pytest.mark.parametrize("filename,generation", [("pk1.json", "1"), ("pk2.json", "2"), ("pk3.json", "3"), ("pk4.json", "4"), ("pk5.json", "5"), ("pk6.json", "6"),  ("pk7.json", "7"), ("pk8.json", "8"), ("pb7.json", "LGPE"), ("pb8.json", "BDSP"), ("pa8.json", "PLA")])
    def test_get_pokemon_summary_with_generation(self, filename, generation):
        """
        Tests creating a pokemon summary
        """
        # Load the expected summary and compare
        with open("tests/jsons/legal/{}".format(filename), mode='r') as expected:
            expected_summary = json.load(expected)
        # Load the pokemon data
        pokemon_data = base64.b64decode(expected_summary["base64"])
        # Load the pokemon data
        pkmn = get_pkmn(pokemon_data, generation)
        assert pkmn is not None
        # Create a summary
        summary = Pokemon(pkmn, self.strings, self.moveTypes, self.sprites)
        
        assert summary is not None
        assert expected_summary == json.loads(summary.toJSON())

    @pytest.mark.parametrize("filename", [("pk1.json"), ("pk2.json"), ("pk3.json"), ("pk4.json"), ("pk5.json"), ("pk6.json"),  ("pk7.json"), ("pk8.json"), ("pb7.json"), ("pb8.json"), ("pa8.json")])
    def test_get_pokemon_summary_without_generation(self, filename):
        """
        Tests creating a pokemon summary
        """
        # Load the expected summary and compare
        with open("tests/jsons/legal/{}".format(filename), mode='r') as expected:
            expected_summary = json.load(expected)
        # Load the pokemon data
        pokemon_data = base64.b64decode(expected_summary["base64"])
        # Load the pokemon data
        pkmn = get_pkmn(pokemon_data)
        assert pkmn is not None
        # Create a summary
        summary = Pokemon(pkmn, self.strings, self.moveTypes, self.sprites)

        assert summary is not None
        assert expected_summary == json.loads(summary.toJSON())
