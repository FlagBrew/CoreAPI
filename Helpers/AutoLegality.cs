using System;
using System.Threading;
using PKHeX.Core;
using PKHeX.Core.AutoMod;
using System.Threading.Tasks;

namespace CoreAPI.Helpers
{
    public class AutoLegality
    {
        private PKM startingPK;
        private CancellationTokenSource cts;
        private GameVersion gv;
        private PKM legalpk;
        private LegalityAnalysis la;
        public bool OkayToRun = false;
        public bool Successful = false;
        public bool Ran = true;
        public string Report;

        static AutoLegality()
        {
            Legalizer.EnableEasterEggs = false;
            APILegality.PrioritizeGame = true;
            APILegality.UseTrainerData = false;
        }

        public AutoLegality(PKM pk, string ver, CancellationTokenSource cancellationTokenSource)
        {
            bool valid = Enum.TryParse<GameVersion>(ver, true, out var game);
            if (valid)
            {
                OkayToRun = true;
                startingPK = pk;
                gv = game;
                cts = cancellationTokenSource;
            }
        }

        public PKM LegalizePokemon()
        {
            return ProcessALM(startingPK, gv);
        }

        private PKM ProcessALM(PKM pkm, GameVersion ver = GameVersion.GP)
        {
            la = new LegalityAnalysis(pkm);
            if (la.Valid)
            {
                legalpk = pkm;
                Ran = false;
                Report = la.Report();
                return legalpk;
            }
            Task.Run (() =>
            {
                var thread = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine($"Legalization on Thread ({thread}) has started at: {DateTime.Now:F}");
                legalpk = Legalize(pkm, ver);
                Console.WriteLine($"Legalization on Thread ({thread}) has finished at: {DateTime.Now:F}");
                cts.Cancel();
            }, cts.Token);
            return legalpk;
        }

        private SimpleTrainerInfo GetTrainerInfo(PKM pk, GameVersion ver)
        {
            return new SimpleTrainerInfo(ver)
            {
                OT = pk.OT_Name,
                SID = pk.SID,
                TID = pk.TID,
                Language = pk.Language,
                Gender = pk.OT_Gender
            };
        }
        private PKM Legalize(PKM pk, GameVersion ver)
        {
            Report = la.Report();
            var sav = SaveUtil.GetBlankSAV(ver, pk.OT_Name);
            sav.TID = pk.TID;
            sav.SID = pk.SID;
            sav.Language = pk.Language;

            PKM upd = sav.Legalize(pk.Clone());
            upd.SetTrainerData(GetTrainerInfo(pk, ver));
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

        public PKM GetLegalPK()
        {
            return legalpk;
        }
    }
}
