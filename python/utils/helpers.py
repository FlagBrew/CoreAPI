#type: ignore ReportMissingImport

from typing import Union
import json
import base64
from PKHeX.Core import PokeList1, PokeList2, PK3, PK4, PK5, PK6, PK7, PK8, PB7, PB8, PA8, GameVersion, GameStrings, SimpleTrainerInfo, EntityFormat

class LanguageStrings:
    __strings = None
    def __init__(self, language: str = "en") -> None:
        self.__strings = GameStrings(language)

    def get_move_name(self, move: int):
        try:
            return self.__strings.movelist[move]
        except:
            return "None"

    def get_species_name(self, species: int):
        try:
            return self.__strings.specieslist[species]
        except:
            return "None"

    def get_ability_name(self, ability: int):
        try:
            return self.__strings.abilitylist[ability]
        except:
            return "None"

    def get_type_name(self, type: int):
        try:
            return self.__strings.types[type]
        except:
            return "None"

    def get_nature_name(self, nature: int):
        try:
            return self.__strings.natures[nature]
        except:
            return "None"
    
    def get_item_name(self, item: int):
        try:
            return self.__strings.itemlist[item]
        except:
            return "None"

    def get_ball_name(self, ball: int):
        try:
            return self.__strings.balllist[ball]
        except:
            return "None"
    
    def get_game_name(self, version: int):
        try:
            return self.__strings.gamelist[version]
        except:
            return "None"

class MoveTypes:
    __types = None
    def __init__(self) -> None:
        # open the JSON file
        with open("data/move_types.json", mode='r') as file:
            self.__types = json.load(file)

    def get_type_name(self, type: str):
        try:
            return self.__types[type]
        except:
            return "None"

def get_pkmn(data, generation = None) -> Union[PokeList1, PokeList2, PK3, PK4, PK5, PK6, PK7, PK8, PB7, PB8, PA8, None]:
    """
    Accepts a bytearray of a PKM file and returns the appropriate PKM object.

    Optionally accepts a generation number to force the PKM object to be of that generation. 

    If no generation is specified, the function will attempt to determine the generation of the PKM file.

    Returns None if the generation cannot be determined or if the provided data is not valid for the specified generation.
    """
    try:
        if generation == "1":
            return PokeList1(data)[0]
        elif generation == "2":
            return PokeList2(data)[0]
        elif generation == "3":
            return PK3(data)
        elif generation == "4":
            return PK4(data)
        elif generation == "5":
            return PK5(data)
        elif generation == "6":
            return PK6(data)
        elif generation == "7":
            return PK7(data)
        elif generation == "8":
            return PK8(data)
        elif generation == "BDSP":
            return PB8(data)
        elif generation == "LGPE":
            return PB7(data)
        elif generation == "PLA":
            return PA8(data)
        elif generation is None:
            # Get the pkmn from both approaches
            entityPkmn = EntityFormat.GetFromBytes(data)
            getPkmnPkmn = get_pkmn(data, get_generation_from_version(entityPkmn.Version))
            # If they are different type, we should check the size
            if str(type(entityPkmn)) != str(type(getPkmnPkmn)):
                # If the entityPkmn is greater than the getPkmnPkmn, we should return the entityPkmn as it is more likely to be correct
                if len(entityPkmn.Data) > len(getPkmnPkmn.Data):
                    return entityPkmn
                # If the sizes are the same, return getPkmnPkmn as EntityFormat.GetFromBytes is more likely to be wrong in situations like BDSP
                return getPkmnPkmn
            else:
                # If they are the same type, we can return entityPkmn as it is more likely to be correct
                return entityPkmn
            # If above doesn't fail, then we can retrieve the generation from the pkmn object using the Version
            # we do this rather than just returning the pokemon directly as legality checks
            # seem to fail if we don't have the correct generation despite the fact that the pokemon is valid
            #return get_pkmn(data, get_generation_from_version(pkmn.Version))
        else:
            return None
    except:
        return None

def get_game_version(pkm):
    return {
        "<class 'PKHeX.Core.PK1'>":  GameVersion.YW,
        "<class 'PKHeX.Core.PK2'>": GameVersion.C,
        "<class 'PKHeX.Core.PK3'>": GameVersion.E,
        "<class 'PKHeX.Core.PK4'>": GameVersion.HG,
        "<class 'PKHeX.Core.PK5'>": GameVersion.B2,
        "<class 'PKHeX.Core.PK6'>": GameVersion.OR,
        "<class 'PKHeX.Core.PK7'>": GameVersion.UM,
        "<class 'PKHeX.Core.PB7'>": GameVersion.GE,
        "<class 'PKHeX.Core.PK8'>": GameVersion.SW,
        "<class 'PKHeX.Core.PB8'>": GameVersion.BD,
        "<class 'PKHeX.Core.PA8'>": GameVersion.PLA,
    }[str(type(pkm))]

def get_trainer_info(pkmn, ver):
    info = SimpleTrainerInfo(ver)
    info.OT = pkmn.OT_Name
    info.SID = pkmn.SID
    info.TID = pkmn.TID
    info.Language = pkmn.Language
    info.Gender = pkmn.Gender
    return info


def get_generation_from_version(ver):
    if ver in [35, 36, 37, 38, 50, 51, 84, 83]:
        return "1"
    elif ver in [39, 40, 41, 52, 53, 85]:
        return "2"
    elif ver in [1, 2, 3, 4, 5, 54, 55, 56, 57, 58, 59]:
        return "3"
    elif ver in [10, 11, 12, 7, 8, 60, 61, 62, 0x3F]:
        return "4"
    elif ver in [20, 21, 22, 23, 0x40, 65]:
        return "5"
    elif ver in [24, 25, 26, 27, 66, 67, 68]:
        return "6"
    elif ver in [30, 0x1F, 0x20, 33, 69, 70]:
        return "7"
    elif ver in [71, 34, 42, 43]:
        return "LGPE"
    elif ver in [44, 45, 47, 72]:
        return "8"
    elif ver in [73, 48, 49]:
        return "BDSP"
    elif ver == 471:
        return "PLA"
    else:
        raise ValueError("Unsuppored game version")

def get_pokemon_from_base64(input, generation = None):
    # decode the base64 string
    try:
        decoded = base64.b64decode(input, validate=True)
    except:
        return None, "Invalid base64 string"
    # get the pkmn object
    pkmn = get_pkmn(decoded, generation)

    if pkmn is None:
        return None, {"error": "Invalid pokemon file or bad generation"}

    return pkmn, None
    