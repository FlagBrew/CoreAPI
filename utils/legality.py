# type: ignore ReportMissingImport

from PKHeX.Core import LegalityAnalysis, LegalityFormatting
from PKHeX.Core.AutoMod import Legalizer, SimpleEdits
import utils.helpers as helpers

def legalize(pkmn):
    info = helpers.get_trainer_info(pkmn, helpers.get_game_version(pkmn))
    legalized = Legalizer.Legalize(info, pkmn)
    SimpleEdits.SetTrainerData(legalized, info)
    report = LegalityAnalysis(legalized)
    if report.Valid:
        return legalized
    return None


def legality_check(pkmn):
    la = LegalityAnalysis(pkmn)
    report = LegalityFormatting.GetLegalityReport(la)

    return la.Valid, report
