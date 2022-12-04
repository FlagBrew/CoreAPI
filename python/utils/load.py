# type: ignore ReportMissingImport

from pythonnet import load
#load("coreclr") <-- Using something other than mono caused sig 11 to occur

import clr, sys, os



sys.path.append(os.getcwd() + r"/deps/")
clr.AddReference("PKHeX.Core")
clr.AddReference("PKHeX.Core.AutoMod")
from PKHeX.Core import GameInfo, EncounterEvent, RibbonStrings
from PKHeX.Core.AutoMod import APILegality

EncounterEvent.RefreshMGDB("")
RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons)
APILegality.EnableEasterEggs = False
APILegality.PrioritizeGame = False
APILegality.UseTrainerData = False
APILegality.Timeout = 15
