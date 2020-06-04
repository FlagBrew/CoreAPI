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
                    var report = la.Report().ToLower();
                    if(!report.Contains("handling trainer"))
                    {
                        if (!report.Contains("untraded"))
                        {
                            info.ApplyToPKM(updated);
                            if (!report.Contains("memory"))
                                updated.HT_Memory = pk.HT_Memory;
                            if (!report.Contains("affection"))
                                updated.HT_Affection = pk.HT_Affection;
                            if (!report.Contains("feeling"))
                                updated.HT_Feeling = pk.HT_Feeling;
                            if (!report.Contains("friendship"))
                                updated.HT_Friendship = pk.HT_Friendship;
                            if (!report.Contains("intensity"))
                                updated.HT_Intensity = pk.HT_Intensity;
                            if (!report.Contains("trash"))
                                updated.HT_Trash = pk.HT_Trash;
                            updated.HT_Gender = pk.HT_Gender;
                            updated.HT_Name = pk.HT_Name;
                            updated.HT_TextVar = pk.HT_TextVar;
                        }
                    } else
                    {
                        updated.OT_Memory = pk.OT_Memory;
                        updated.OT_Friendship = pk.OT_Friendship;
                        updated.OT_Name = pk.OT_Name;
                        updated.OT_Affection = pk.OT_Affection;
                        updated.OT_Feeling = pk.OT_Feeling;
                        updated.OT_Gender = pk.OT_Gender;
                        updated.OT_Intensity = pk.OT_Intensity;
                        updated.OT_TextVar = pk.OT_TextVar;
                        updated.OT_Trash = pk.OT_Trash;
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
