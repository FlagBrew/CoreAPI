namespace coreconsole.Models;

public struct EggData
{
    public string Name { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }

    public EggData(string name, int year, int month, int day)
    {
        Name = name;
        Year = year;
        Month = month;
        Day = day;
    }
}