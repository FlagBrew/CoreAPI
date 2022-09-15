# type: ignore ReportMissingImport

import clr, sys, os

sys.path.append(os.getcwd() + r"/deps/")
clr.AddReference("PKHeX.Core")
clr.AddReference("PKHeX.Core.AutoMod")
from PKHeX.Core import GameInfo, EncounterEvent, RibbonStrings

EncounterEvent.RefreshMGDB("")
RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons)
