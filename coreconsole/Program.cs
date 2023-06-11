// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.Text.Json;
using coreconsole.handlers;
using coreconsole.utils;
using PKHeX.Core;
using Sentry;
using Version = coreconsole.Models.Version;

namespace coreconsole;

public static class MainClass
{
    public static void Main(string[] args)
    {
        if(!Helpers.LoadEnv()) return;
        
        using (SentrySdk.Init(o =>
               {
                   o.Dsn = Environment.GetEnvironmentVariable("SENTRY_DSN");
               }))
        {
                 var pokemonArg = new Argument<string>(
            "pokemon",
            "The pokemon file (in base64 format)."
        );

        var generationOption = new Option<EntityContext?>(
            "--generation",
            "Used to determine desired generation when generation could be 6/7 or 8/8b"
        );

        var cmd1 = new Command("summary", "Returns the summary for a given pokemon.")
        {
            pokemonArg,
            generationOption
        };
        cmd1.SetHandler(Summary.SummaryHandler, pokemonArg, generationOption);

        var cmd2 = new Command("legality", "Returns the legality status for a Pokemon, including what checks fail.")
        {
            pokemonArg,
            generationOption
        };

        cmd2.SetHandler(Legality.LegalityCheckHandler, pokemonArg, generationOption);

        var legalizationGenerationOverride = new Option<int?>("--legalization-generation",
            "Forces the legalized Pokemon to use the provided generation (may cause legalization to fail).");

        var legalizationGameVersionOverride = new Option<GameVersion?>("--version",
            "Game Version to use in trying to legalize the Pokemon (may cause legalization to fail).");

        var cmd3 = new Command("legalize", "Attempts to auto legalize a pokemon and returns it if successful.")
        {
            pokemonArg,
            generationOption,
            legalizationGenerationOverride,
            legalizationGameVersionOverride
        };
        cmd3.SetHandler(Legality.LegalizeHandler, pokemonArg, generationOption,
            legalizationGenerationOverride, legalizationGameVersionOverride);

        var cmd4 = new Command("version", "Returns the version for ALM/PKHeX");
        cmd4.SetHandler(() =>
        {
           Console.WriteLine(JsonSerializer.Serialize(new Version()));
        });

        var cli = new RootCommand("CoreConsole - a tool for interacting with PKHeX and Auto Legality via CLI.")
        {
            cmd1,
            cmd2,
            cmd3,
            cmd4
        };
        cli.Invoke(args);   
        }
    }
}