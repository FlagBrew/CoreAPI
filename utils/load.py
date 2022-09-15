# type: ignore ReportMissingImport

import clr, sys, os

sys.path.append(os.getcwd() + r"/deps/")
clr.AddReference("PKHeX.Core")
clr.AddReference("PKHeX.Core.AutoMod")
from PKHeX.Core import GameInfo, EncounterEvent, RibbonStrings
from PKHeX.Core.AutoMod import APILegality

EncounterEvent.RefreshMGDB("")
RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons)
APILegality.EnableEasterEggs = True
APILegality.PrioritizeGame = True;
APILegality.UseTrainerData = True;
