# type: ignore ReportMissingImport

import random
from utils.helpers import LanguageStrings
from PKHeX.Core import FormConverter, GameInfo, EntityContext
import json

class Sprites:
    BASE_URL = "https://cdn.sigkill.tech/sprites/"
    POKEMON_WITH_FEMALE_FORMS = ['frillish','hippopotas','hippowdon','jellicent','meowstic','pikachu','pyroar','unfezant','wobbuffet','basculegion','indeedee']
    REPLACE_CHARS = {
        "♀": "f",
        "♂": "m",
        "é": "e",
        "’": "",
        "'": "",
        ": ": "-",
        " ": "-",
        ".": "",
    }
    ALCREMIE_DECORATIONS = {
        "0": "strawberry",
        "1": "berry",
        "2": "love",
        "3": "star",
        "4": "clover",
        "5": "flower",
        "6": "ribbon",
    }
    
    _bindings = {}
    _item_bindings = {}

    def __init__(self):
        with open("data/bindings.json", mode='r') as file:
            self._bindings = json.load(file)
        with open("data/item-map.json", mode='r') as file:
            self._item_bindings = json.load(file)
    
    def get_pokemon_sprite(self, species, gender, shiny, form, useGen8Sprites, formArg = None):
        # Check to see if the species is in the POKEMON_WITH_FEMALE_FORMS
        checkBinding = True
        path = "{}{}{}".format(self.BASE_URL, "pokemon-gen7x/" if not useGen8Sprites else "pokemon-gen8/", "shiny/" if shiny else "regular/")

        if species in self.POKEMON_WITH_FEMALE_FORMS and gender == "female":
            path += "female/"
            if species == "pikachu" and form == "normal":
                checkBinding = False
        
        if checkBinding:
            lookup = "{}_{}".format(species.replace(' ', '_'), form.replace(' ', '_'))
            binding = self._bindings.get(lookup, None)
            if binding is not None:
                path += binding['file']
                if species == "alcremie":
                    path = path.replace('.png', '-{}.png'.format(self.ALCREMIE_DECORATIONS.get(str(formArg), "strawberry")))
                return path
        # Check to see if we need to replace any characters
        for char in self.REPLACE_CHARS:
            species = species.replace(char, self.REPLACE_CHARS[char])
        path += "{}.png".format(species)
    
        return path

    def get_item_sprite(self, item):
        # convert the item to a string
        item = str(item)
        # check the length of the item, we want 0000 format
        if len(item) < 4:
            item = item.zfill(4)
        
        # check to see if the item is in the item map
        binding = self._item_bindings.get("item_{}".format(item), None)
        if binding is not None:
            return "{}items/{}.png".format(self.BASE_URL, binding)
        # if not, return None
        return None


def generate_bindings(strings: LanguageStrings):
    contexts = [
        EntityContext.Gen1,
        EntityContext.Gen2,
        EntityContext.Gen3,
        EntityContext.Gen4,
        EntityContext.Gen5,
        EntityContext.Gen6,
        EntityContext.Gen7,
        EntityContext.Gen7b,
        EntityContext.Gen8,
        EntityContext.Gen8a,
        EntityContext.Gen8b
    ]
    species_form = []
    has_female_forms = [
        'frillish',
        'hippopotas',
        'hippowdon',
        'jellicent',
        'meowstic',
        'pikachu',
        'pyroar',
        'unfezant',
        'wobbuffet',
        'basculegion',
        'indeedee'
    ]
    for context in contexts:
        for i in range(1, 905):
            forms = FormConverter.GetFormList(i, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, context)
            species = strings.get_species_name(i).lower()
            for form in forms:
                form = form.lower()
                if form == "":
                    if species in has_female_forms and species not in species_form:
                        species_form.append("{}".format(species))    
                    continue
                if form == "normal" and species not in species_form:
                    species_form.append("{}".format(species))
                    continue
                elif form == "normal":
                    continue
                if form == "!":
                    form = "exclamation"
                if form == "?":
                    form = "question"
                if form == "♂":
                    form = "m"
                if form == "♀":
                    form = "f"

                if "{} {}".format(species, form) not in species_form:
                    species_form.append("{} {}".format(species, form))
    species_form.sort()
    # check to see if bindings.json exists, if it does check to see if we have a lastIndex
    lastIndex = 0
    try:
        with open("bindings.json", mode='r') as bindings:
            data = json.load(bindings)
            lastIndex = data.get("last_index")
            # check the size
            if len(species_form) != data.get('last_size'):
                print(len(species_form), data.get('last_size'))
                q = input("The size of the species_form list is different than the lastSize in bindings.json, do you want to start from the beginning? (y/n): ")
                if q == "y":
                    lastIndex = 0
    except FileNotFoundError:
        pass

    print("There are {} forms to go through and you have gotten through {} of them, meaning {} more to go, good luck, you're doing this manually".format(len(species_form), lastIndex, len(species_form)- lastIndex))
    bindings = {}
    if lastIndex != 0:
        bindings = data.get('bindings')
    try:
        for s in species_form:
            if lastIndex != 0:
                lastIndex -= 1
                continue
            splitted = s.split(" ")
            species = splitted[0]
            has_female = species in has_female_forms
            if len(splitted) == 1:
                continue

            form = "-".join(splitted[1:])

            # prompt for the image filename
            question = input("is the image for {0}'s {1} form named {0}-{1}.png? (y/n): ".format(species, form))
            if question == "y":
                bindings[s.replace(" ", "_")] = {
                    "file": "{}-{}.png".format(species, form),
                    "hasGen7Naming": False,
                    "hasFemale": has_female
                }
                continue
            question2 = input("is it literally just the damn species name? (y/n): ")
            if question2 == "y":
                bindings[s.replace(" ", "_")] = {
                    "file": "{}.png".format(species),
                    "hasGen7Naming": False,
                    "hasFemale": has_female
                }
                continue
            name = input("Enter the filename for {} (DO NOT ENTER THE EXTENSION, ASSUMING IT IS PNG): ".format(s))
            hasGen7 = input("Does this have a -gen7 form? (y or blank): ")
            if hasGen7 == "y":
                hasGen7 = True
            else:
                hasGen7 = False
        
            bindings[s.replace(" ", "_")] = {
                "file": "{}.png".format(name),
                "hasGen7Naming": hasGen7,
                "hasFemale": has_female
            }

        # Write the bindings to a json file
        with open("bindings.json", mode='w') as f:
            json.dump(bindings, f, indent=4)
    except KeyboardInterrupt:
        # Write the bindings to a json file so we can resume later
        with open("bindings.json", mode='w') as f:
            json.dump({
                "bindings": bindings,
                "last_index": species_form.index(s),
                "last_size": len(species_form)
            }, f, indent=4)
        print("Interrupted, saved bindings to bindings.json")

    
        
        