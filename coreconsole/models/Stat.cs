using System.Text.Json.Serialization;

namespace coreconsole.Models;

public struct Stat
{
    public Stat(string name, int iv, int ev, string total)
    {
        Name = name;
        IV = iv;
        EV = ev;
        Total = total;
    }

    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("iv")] public int IV { get; set; }
    [JsonPropertyName("ev")] public int EV { get; set; }
    [JsonPropertyName("total")] public string Total { get; set; }
}