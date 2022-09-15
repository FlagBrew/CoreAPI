# type: ignore ReportMissingImport

from utils.load import * # This handles the loading of the c# dlls

from System import Enum 
from System.Collections.Generic import List
from PKHeX.Core import EncounterLearn, Species, SaveUtil, GameVersion, PB8, LegalityAnalysis, LegalityFormatting, EntitySummary, GameStrings, EntityContext, FormConverter, GameInfo, CommonEdits, EncounterEvent, RibbonStrings
from PKHeX.Core.AutoMod import Legalizer, SimpleEdits
import utils.helpers as helpers
from utils.pkmn import Pokemon
from utils.sprites import Sprites

def testing():
    with open("493 - Arceus - 0FB4692793C5.pa8", mode='rb') as test:
        data = bytearray(test.read())
    pk = helpers.get_pkmn(data, "PLA")
    if pk is None:
        print("PK is None")
        exit(1)
    moveTypes = helpers.MoveTypes()
    j = Pokemon(pk, helpers.LanguageStrings(), moveTypes, Sprites()).toJSON()
    print(j)

if __name__ == "__main__":
    testing()
    # test_forms()
    # placeholder_get_info(helpers.LanguageStrings())
