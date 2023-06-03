using coreconsole.Models;
using PKHeX.Core;

namespace coreconsole.handlers;

public class Summary
{
    public Pokemon GetSummary(PKM pokemon)
    {
        var pes = new PublicEntitySummary(pokemon, GameInfo.Strings);
        var pkmn = new Pokemon(pokemon, pes);

        return pkmn;
    }
}