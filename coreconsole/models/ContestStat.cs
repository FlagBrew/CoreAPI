namespace coreconsole.Models;

public struct ContestStat
{
    public ContestStat(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; }
    public int Value { get; set; }
}