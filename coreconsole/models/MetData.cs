namespace coreconsole.Models;

public struct MetData
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }

    public MetData(string name, int level, int year, int month, int day)
    {
        Name = name;
        Level = level;
        Year = year;
        Month = month;
        Day = day;
    }
}