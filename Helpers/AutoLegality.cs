using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;
using PKHeX.Core;
using PKHeX.Core.AutoMod;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CoreAPI.Helpers
{

    public class AutoLegality
    {
        private PKM startingPK;
        private CancellationTokenSource cts;
        private GameVersion gv;
        private static PKM legalpk;
        private static LegalityAnalysis la;
        private static bool Initialized = false;
        public bool OkayToRun = false;
        public bool Successful = false;
        public bool Ran = true;
        public string Report;

        public static void EnsureInitialized()
        {
            if (Initialized)
            {
                return;
            }
            Initalize();
        }

        private static void Initalize()
        {
            Legalizer.AllowBruteForce = false;
            Legalizer.EnableEasterEggs = false;
            Legalizer.AllowAPI = true;
            APILegality.PrioritizeGame = true;
            APILegality.UseTrainerData = false;
            Initialized = true;
        }

        public AutoLegality(PKM pk, string ver, CancellationTokenSource cancellationTokenSource)
        {
            EnsureInitialized();
            bool valid = Enum.TryParse<GameVersion>(ver, true, out var game);
            if (valid)
            {
                OkayToRun = true;
                startingPK = pk;
                gv = game;
                cts = cancellationTokenSource;
            }
            return;
        }

        public PKM LegalizePokemon()
        {
            return ProcessALM(startingPK, gv);
        }

        private PKM ProcessALM(PKM pkm, GameVersion ver = GameVersion.GP)
        {
            la = new LegalityAnalysis(pkm);
            var tcs = new TaskCompletionSource<string>();
            if (la.Valid)
            {
                legalpk = pkm;
                Ran = false;
                Report = la.Report();
                return legalpk;
            }
            Task.Run (() =>
            {
                Console.WriteLine(String.Format("Legalization on Thread ({0}) has started at: {1}", Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToString("F")));
                legalpk = Legalize(pkm, ver);
                Console.WriteLine(String.Format("Legalization on Thread ({0}) has finished at: {1}", Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToString("F")));
                cts.Cancel();
            }, cts.Token);
            return legalpk;
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

            PKM upd = sav.Legalize(pk.Clone());
            upd.SetTrainerData(getInfo(pk, ver));
            la = new LegalityAnalysis(upd);
            if (la.Valid)
            {
                legalpk = upd;
                Successful = true;
                return legalpk;
                //Report = la.Report();
            }
            return null;
        }

        public PKM getLegalPK()
        {
            return legalpk;
        }
    }
}
