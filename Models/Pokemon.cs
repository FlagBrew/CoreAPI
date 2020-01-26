using PKHeX.Core;
using CoreAPI.Helpers;

namespace CoreAPI.Models
{
    public class Pokemon
    {
        public PokemonSummary Summary { get; }
        public Pokemon(PKM pk)
        {
           Summary = new PokemonSummary(pk, GameInfo.Strings);
        }

    }
}
