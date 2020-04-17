using System;
using System.Threading;
using PKHeX.Core;
using PKHeX.Core.AutoMod;
namespace CoreAPI.Helpers
{

    public class AutoLegality
    {
        private static PKM legalpk;
        private static LegalityAnalysis la;
        private static bool Initialized;
        public bool Successful = true;
        public bool Ran = true;
        public string Report;

        public static void EnsureInitialized()
        {
            if (Initialized)
            {
                return;
            }
            Initialized = true;
            Initalize();
        }

        public static void Initalize()
        {
            //InitializeCoreStrings();
            Legalizer.AllowBruteForce = true;
        }

            public AutoLegality(PKM pk, string ver)
        {
            EnsureInitialized();
            bool valid = Enum.TryParse<GameVersion>(ver, true, out var game);
            if (valid)
                ProcessALM(pk, game);
            return;
        }

        private static void InitializeCoreStrings()
        {
            var lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Substring(0, 2);
            Util.SetLocalization(typeof(LegalityCheckStrings), lang);
            Util.SetLocalization(typeof(MessageStrings), lang);
            RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons);

            // Update Legality Analysis strings
            LegalityAnalysis.MoveStrings = GameInfo.Strings.movelist;
            LegalityAnalysis.SpeciesStrings = GameInfo.Strings.specieslist;
        }

        private void ProcessALM(PKM pkm, GameVersion ver = GameVersion.GP)
        { 
            la = new LegalityAnalysis(pkm);
            if (la.Valid)
            {
                legalpk = pkm;
                Ran = false;
                Report = la.Report();
                return;
            }
            if (la.Report().ToLower().Contains("invalid move")){
                Ran = true; // because piepie62 and griffin wanted to make my program a liar. GG guys GG.
                Successful = false;
                Report = la.Report();
                return;
            }
            legalpk = Legalize(pkm, ver);
        }

        private SimpleTrainerInfo getInfo(PKM pk, GameVersion ver)
        {
            SimpleTrainerInfo info = new SimpleTrainerInfo(ver);
            info.OT = pk.OT_Name;
            info.SID = pk.SID;
            info.TID = pk.TID;
            info.Language = pk.Language;
            info.SubRegion = pk.Region;
            info.Country = pk.Country;
            info.ConsoleRegion = pk.ConsoleRegion;
            info.Gender = pk.OT_Gender;
            return info;
        }

        private PKM Legalize(PKM pk, GameVersion ver)
        {
            var KeepOriginalData = true;
            Successful = false;
            SimpleTrainerInfo info = getInfo(pk, ver);

            if (la.Report().ToLower().Contains("wordfilter") || la.Report().Contains("SID") || la.Report().Contains("TID"))
            {
                KeepOriginalData = false;
            }
            Legalizer.AllowBruteForce = true;
            Legalizer.AllowAPI = true;
            var timeout = TimeSpan.FromSeconds(5);
            var started = DateTime.UtcNow;
            PKM updated;
            var thread = new Thread(() => {
                if (KeepOriginalData)
                {
                    updated = Legalizer.Legalize(pk);
                    info.ApplyToPKM(updated);
                    if(!la.Report().Contains("Handling Trainer"))
                    {
                        updated.HT_Affection = pk.HT_Affection;
                        updated.HT_Feeling = pk.HT_Feeling;
                        updated.HT_Friendship = pk.HT_Friendship;
                        updated.HT_Gender = pk.HT_Gender;
                        updated.HT_Intensity = pk.HT_Intensity;
                        updated.HT_Memory = pk.HT_Memory;
                        updated.HT_Name = pk.HT_Name;
                        updated.HT_TextVar = pk.HT_TextVar;
                        updated.HT_Trash = pk.HT_Trash;
                    }
                } else
                {
                    updated = Legalizer.Legalize(pk);
                }
                if (new LegalityAnalysis(updated).Valid)
                {
                    legalpk = updated;
                    Successful = true;
                    Report = la.Report();
                }
            });
            thread.Start();
            while (thread.IsAlive && DateTime.UtcNow - started < timeout)
            {
                Thread.Sleep(100);
            }
            if (thread.IsAlive)
                thread.Abort();
          
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
