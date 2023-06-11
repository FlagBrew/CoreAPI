using System.Text.Json.Serialization;

namespace coreconsole.Models;

public struct Version
{
    [JsonPropertyName("alm_version")]
    public string? AlmVersion { get; set; }
    [JsonPropertyName("pkhex_version")]
    public string? PkHeXVersion { get; set; }
    
    public Version()
    {
        AlmVersion = PKHeX.Core.AutoMod.ALMVersion.Versions.AlmVersionCurrent?.ToString();
        PkHeXVersion = PKHeX.Core.AutoMod.ALMVersion.Versions.CoreVersionLatest?.ToString();
    }
}