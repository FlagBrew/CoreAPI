using System.Text.Json.Serialization;

namespace coreconsole.Models;

public struct ContestStat
{
    public ContestStat(string name, int value)
    {
        Name = name;
        Value = value;
    }

    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("value")] public int Value { get; set; }
}