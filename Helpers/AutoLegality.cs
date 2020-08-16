using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;
using PKHeX.Core;
using PKHeX.Core.AutoMod;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;

namespace CoreAPI.Helpers
{

    public class AutoLegality
    {
        private static PKM legalpk;
        private static LegalityAnalysis la;
        private static bool Initialized;
        private readonly Random _random = new Random();
        public bool Successful = false;
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
            /*if (la.Report().ToLower().Contains("invalid move")){
                Ran = true; // because piepie62 and griffin wanted to make my program a liar. GG guys GG.
                Successful = false;
                Report = la.Report();
                return;
            }*/
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
            Report = la.Report();
            var sav = SaveUtil.GetBlankSAV(ver, pk.OT_Name);
            sav.TID = pk.TID;
            sav.SID = pk.SID;
            sav.Language = pk.Language;
            Legalizer.AllowBruteForce = true;
            Legalizer.EnableEasterEggs = false;
            Legalizer.AllowAPI = true;
            APILegality.PrioritizeGame = true;
            APILegality.UseTrainerData = false;
            PKM upd = sav.Legalize(pk.Clone());
            upd.SetTrainerData(getInfo(pk, ver));
            la = new LegalityAnalysis(upd);
            if(la.Valid)
            {
                legalpk = upd;
                Successful = true;
                //Report = la.Report();
            }
            else
            {
                upd = sav.Legalize(pk.Clone());
                la = new LegalityAnalysis(upd);
                if (la.Valid)
                {
                    legalpk = upd;
                    Successful = true;
                } else
                {
                    Console.WriteLine(la.Report());
                }
            }

            if (Successful)
            {
                return legalpk;
            }

            return null;
        }
        public PKM GetLegalPKM()
        {
            return legalpk;
        }
    }
}
