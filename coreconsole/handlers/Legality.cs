using System.Text.Json;
using coreconsole.Models;
using coreconsole.utils;
using PKHeX.Core;
using PKHeX.Core.AutoMod;

namespace coreconsole.handlers;

public static class Legality
{
    private static PKM? MainHandler(string pokemon, EntityContext? context)
    {
        var pkmn = Helpers.PokemonFromBase64(pokemon, context ?? EntityContext.None);
        if (pkmn == null) return null;
        Helpers.Init();

        return pkmn;
    }

    public static void LegalityCheckHandler(string pokemon, EntityContext? context)
    {
        var pkmn = MainHandler(pokemon, context);
        if (pkmn == null) return;

        var la = CheckLegality(pkmn);
        Console.WriteLine(JsonSerializer.Serialize(new LegalityCheckReport(la), Helpers.SerializerOptions));
    }

    public static void LegalizeHandler(string pokemon, EntityContext? context, int? generation, GameVersion? version)
    {
        var pkmn = MainHandler(pokemon, context);
        if (pkmn == null) return;

        var report = CheckLegality(pkmn);
        if (report.Valid)
        {
            Console.WriteLine("{\"error\": \"this pokemon is already legal!\"}");
            return;
        }
        var result = AutoLegalize(pkmn, generation, version);
        if (result != null)
        {
            report = CheckLegality(result);
        }
        Console.WriteLine(JsonSerializer.Serialize(new AutoLegalizationResult(report, result), Helpers.SerializerOptions));
    }

    public static LegalityAnalysis CheckLegality(PKM pokemon)
    {
        return new LegalityAnalysis(pokemon);
    }

    public static PKM? AutoLegalize(PKM pokemon, int? overriddenGeneration = null,
        GameVersion? overriddenVersion = null)
    {
        var version = overriddenVersion ?? (Enum.TryParse(pokemon.Version.ToString(), out GameVersion parsedVersion)
            ? parsedVersion
            : null);
        var info = _GetTrainerInfo(pokemon, version);
        var regenTemplate = new RegenTemplate(pokemon, overriddenGeneration ?? info.Generation);

        var pkmn = info.GetLegalFromTemplateTimeout(pokemon, regenTemplate, out var result);
        if (result != LegalizationResult.Regenerated) return null;

        pkmn.SetTrainerData(info);

        // Check if still legal
        return !CheckLegality(pkmn).Valid ? null : pkmn;
    }

    private static SimpleTrainerInfo _GetTrainerInfo(PKM pokemon, GameVersion? version)
    {
        return new SimpleTrainerInfo(version ?? GameVersion.SL)
        {
            OT = pokemon.OT_Name,
            SID16 = pokemon.SID16,
            TID16 = pokemon.TID16,
            Language = pokemon.Language,
            Gender = pokemon.OT_Gender
        };
    }
}