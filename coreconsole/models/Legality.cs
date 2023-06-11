using System.Text.Json.Serialization;
using PKHeX.Core;
using Sentry;

namespace coreconsole.Models;

public struct LegalityCheckReport
{
    [JsonPropertyName("legal")] public bool Legal { get; set; }
    [JsonPropertyName("report")] public string[] Report { get; set; }

    public LegalityCheckReport(LegalityAnalysis la)
    {
        Legal = la.Valid;
        Report = la.Report().Split("\n");
    }
}

public struct AutoLegalizationResult
{
    [JsonPropertyName("legal")] public bool Legal { get; set; }
    [JsonPropertyName("report")] public string[] Report { get; set; }
    [JsonPropertyName("pokemon")] public string? PokemonBase64 { get; set; }

    public AutoLegalizationResult(LegalityAnalysis la, PKM? pokemon)
    {
        Legal = la.Valid;
        Report = la.Report().Split("\n");

        if (pokemon == null) return;
        try
        {
            PokemonBase64 = Convert.ToBase64String(pokemon.SIZE_PARTY > pokemon.SIZE_STORED
                ? pokemon.DecryptedPartyData
                : pokemon.DecryptedBoxData);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
        }
    }
}