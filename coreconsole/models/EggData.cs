using System.Text.Json.Serialization;

namespace coreconsole.Models;

public struct EggData
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("year")] public int Year { get; set; }
    [JsonPropertyName("month")] public int Month { get; set; }
    [JsonPropertyName("day")] public int Day { get; set; }

    public EggData(string name, int year, int month, int day)
    {
        Name = name == "(None)" ? "None" : name;
        Year = year;
        Month = month;
        Day = day;
    }
}