using System.Text.Json;
using coreconsole.Models;
using coreconsole.utils;
using PKHeX.Core;

namespace coreconsole.handlers;

public class Summary
{
    public static void SummaryHandler(string pokemon, EntityContext? context)
    {
        var pkmn = Helpers.PokemonFromBase64(pokemon, context ?? EntityContext.None);
        if (pkmn == null) return;
        Helpers.Init();
        var summary = GetSummary(pkmn);
        Console.WriteLine(JsonSerializer.Serialize(summary, Helpers.SerializerOptions));
    }

    public static Pokemon GetSummary(PKM pokemon)
    {
        var pes = new PublicEntitySummary(pokemon, GameInfo.Strings);
        var pkmn = new Pokemon(pokemon, pes);

        return pkmn;
    }
}