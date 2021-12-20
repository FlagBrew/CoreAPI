using System;
using System.Buffers.Text;
using System.Threading;
using CoreAPI.Helpers;
using PKHeX.Core;
using Sentry;

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

        public void ass(string abc)
        {

        }

        public Legalize(PKM pk, string version)
        {
            CancellationTokenSource cts = new CancellationTokenSource(10000);
            try
            {
                var al = new AutoLegality(pk, version, cts);
                if (al.OkayToRun)
                {
                    PKM pkmn;
                    Thread thread = new Thread(() => {
                        al.LegalizePokemon(cts);
                    });
                    thread.Start();
                    while (true)
                    {
                        if (cts.IsCancellationRequested)
                        {
                            thread.Interrupt();
                            pkmn = al.GetLegalPK();
                            break;
                        }
                        Thread.Sleep(100);
                    }
                    Success = al.Successful;
                    Report = al.Report.Split('\n');
                    Ran = al.Ran;
                    if (Success)
                    {
                        try
                        {
                            Pokemon = Convert.ToBase64String(pkmn.DecryptedBoxData);
                            Species = new PokemonSummary(pkmn, GameInfo.Strings).Species;
                            try
                            {
                                QR = Utils.GenerateQR(QRMessageUtil.GetMessage(pkmn));
                            }
                            catch
                            {
                                QR = "";
                            }
                        }
                        catch
                        {
                            Pokemon = "";
                            Species = "";
                            Success = false;
                            Ran = true;
                            Report = new[] { "Stuck in legalization!" };
                        }
                    }
                    else
                    {
                        Pokemon = "";
                    }
                }
                else
                {
                    Ran = false;
                    Success = false;
                    Report = new[] { "Could not run legalization!" };
                }
            }
            catch (Exception e)
            {
                cts.Cancel();
                if(SentrySdk.IsEnabled)
                {
                    SentrySdk.ConfigureScope(scope =>
                    {
                        scope.Contexts["pokemon"] = new
                        {
                            Version = version,
                            Base64 = Convert.ToBase64String(pk.DecryptedBoxData)
                        };
                    });
                    SentrySdk.CaptureException(e);
                }
            }
        }
    }
}
