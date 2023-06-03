using PKHeX.Core;

namespace coreconsole.handlers;

public class Legality
{
    public static LegalityAnalysis CheckLegality(PKM pokemon)
    {
        return new LegalityAnalysis(pokemon);
    }
}