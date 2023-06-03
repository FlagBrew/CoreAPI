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

    public string Name { get; set; }
    public int IV { get; set; }
    public int EV { get; set; }
    public string Total { get; set; }
}