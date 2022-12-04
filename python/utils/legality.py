# type: ignore ReportMissingImport

from PKHeX.Core import LegalityAnalysis, LegalityFormatting
from PKHeX.Core.AutoMod import SimpleEdits, RegenTemplate, LegalizationResult, APILegality
import utils.helpers as helpers
from multiprocessing import Process, Queue
import time
import base64


x = None

def autolegalityThead(info, pkmn, set, legalization_res, out):
    pkmn, _ = APILegality.GetLegalFromTemplate(info, pkmn, set, legalization_res)
    out.put(base64.b64encode(bytearray(byte for byte in pkmn.DecryptedBoxData)).decode('UTF-8'))

def cancel():
    if x is not None:
        x.kill()

def legalize(pkmn, generation):
    global x
    info = helpers.get_trainer_info(pkmn, helpers.get_game_version(pkmn))
    set = RegenTemplate(pkmn, info.Generation)
    legalization_res = LegalizationResult(0)
    out = Queue()

    x = Process(target=autolegalityThead, args=(info, pkmn, set, legalization_res, out))
    x.daemon = True
    x.start()

    i = 0
    killed = False
    while x.is_alive():
        time.sleep(1)
        i += 1
        if i > 15:
            killed = True
            x.kill()
    if killed:
        return None

    # convert the pokemon back
    result = out.get()

    legalized, err = helpers.get_pokemon_from_base64(result, generation)
    if err is not None:
        print(err, info.Generation)
        return None
    
    SimpleEdits.SetTrainerData(legalized, info)
    report = LegalityAnalysis(legalized)
    if report.Valid:
        return legalized
    return None


def legality_check(pkmn):
    la = LegalityAnalysis(pkmn)
    report = LegalityFormatting.GetLegalityReport(la)

    return la.Valid, report
