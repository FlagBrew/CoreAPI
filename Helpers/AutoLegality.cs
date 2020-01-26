using System;
using PKHeX.Core;
using PKHeX.Core.AutoMod;
namespace CoreAPI.Helpers
{

    public class AutoLegality
    {
        private static PKM legalpk;
        private static LegalityAnalysis la;
        public bool Successful = true;
        public bool Ran = true;
        public string Report;

        public AutoLegality(PKM pk, string ver)
        {
            bool valid = Enum.TryParse<GameVersion>(ver, true, out var game);
            if (valid)
                ProcessALM(pk, game);
            return;
        }

        private void ProcessALM(PKM pkm, GameVersion ver = GameVersion.GP)
        {
            la = new LegalityAnalysis(pkm);
            if (la.Valid)
            {
                legalpk = pkm;
                Ran = false;
                Report = la.Report();
            }
            else
                legalpk = Legalize(pkm, ver);
        }

        private PKM Legalize(PKM pk, GameVersion ver)
        {
            var OriginlTrainer = pk.OT_Name;
            var HandlingTrainer = pk.HT_Name;
            var KeepOriginalData = true;
            Successful = false;
            if (la.Report().ToLower().Contains("wordfilter") || la.Report().Contains("SID") || la.Report().Contains("TID"))
            {
                KeepOriginalData = false;
                HandlingTrainer = "PKHeX";
            }
            var sav = SaveUtil.GetBlankSAV(ver, HandlingTrainer);
            var updated = sav.Legalize(pk);
            // These are the data that PKHEX AutoMod sets by itself
            var NewSID = updated.SID;
            var NewOT = updated.OT_Name;
            var NewTID = updated.TID;

            if (KeepOriginalData)
            {
                updated.TID = pk.TID;
                updated.SID = pk.SID;
                updated.OT_Name = OriginlTrainer;
            }
            if (new LegalityAnalysis(updated).Valid)
            {
                legalpk = updated;
                Successful = true;
                Report = la.Report();
            } else
            {
                // Let's try reverting to the old name data
                if (KeepOriginalData)
                {
                    updated.TID = NewTID;
                    updated.OT_Name = NewOT;
                    updated.SID = NewSID;
                    // Let's trying running the LegalityAnalysis again
                    if(new LegalityAnalysis(updated).Valid)
                    {
                        legalpk = updated;
                        Successful = true;
                        Report = la.Report();
                    }
                }
            }
            if (Successful)
            {
                return legalpk;
            } else
            {
                Report = la.Report();
            }
            return null;
        }
        public PKM GetLegalPKM()
        {
            return legalpk;
        }
    }
}
