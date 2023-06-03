// See https://aka.ms/new-console-template for more information

using coreconsole.Models;
using PKHeX.Core;

namespace coreconsole;

public static class MainClass
{
    private static void Init()
    {
        EncounterEvent.RefreshMGDB(string.Empty);
        RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons);
        Sprites.Init();
        // var a = File.ReadAllText("./data/move_types.json");
        // Move.MoveTypes = JsonSerializer.Deserialize<List<MoveType>>(a);
    }

    public static void Main(string[] args)
    {
        Init();
    }
}