using System.Text.Json.Serialization;

namespace coreconsole.Models;

public struct MetData
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("level")] public int Level { get; set; }
    [JsonPropertyName("year")] public int Year { get; set; }
    [JsonPropertyName("month")] public int Month { get; set; }
    [JsonPropertyName("day")] public int Day { get; set; }

    public MetData(string name, int level, int year, int month, int day)
    {
        Name = name == "(None)" ? "None" : name;
        Level = level;
        Year = year;
        Month = month;
        Day = day;
    }
}