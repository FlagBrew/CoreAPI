using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreAPI.Helpers;
using PKHeX.Core;

namespace CoreAPI.Models
{
    public class Legalize
    {
        public string Pokemon { get; }
        public bool Success { get; }
        public string Species { get; }
        public string[] Report { get; }
        public bool Ran { get; }
        public string QR { get; }

        public Legalize(PKM pk, string version)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var al =  new AutoLegality(pk, version);
            Success = al.Successful;
            Report = al.Report.Split('\n');
            Ran = al.Ran;
            if (Success)
            {
                Pokemon = Convert.ToBase64String(al.GetLegalPKM().DecryptedBoxData);
                Species = new PokemonSummary(al.GetLegalPKM(), GameInfo.Strings).Species;
                try
                {
                  QR = Utils.GenerateQR(QRMessageUtil.GetMessage(al.GetLegalPKM()));
                } catch
                {
                    QR = "";
                }
            } else
            {
                Pokemon = "";
            }
        }
    }
}
