using coreconsole.handlers;
using coreconsole.utils;
using PKHeX.Core;

namespace Tests;

[TestFixture]
public class AutoLegalityTest
{
    [SetUp]
    public void Setup()
    {
        Helpers.Init();
    }

    [Test]
    public void CanAutoLegalizePokemon()
    {
        const string pkmnHex =
            "1a9b12b00000701626020000537e0c70d8000000467800020000000000000000000000000000000021003700fd00000023190a0000000000b9227415000000000a13000000000000420061007300630075006c0069006e00ffff0000ffff001400000000000000004400650073006d0075006e006400ffff00000017021000000e00000406000000";

        var pkmnBytes = Helpers.StringToByteArray(pkmnHex);
        var pkmn = EntityFormat.GetFromBytes(pkmnBytes);
        Assert.That(pkmn, Is.Not.Null);
        // Check the legality first
        var legalityReport = Legality.CheckLegality(pkmn!);
        Assert.That(legalityReport.Valid, Is.False);

        // Now try to auto legalize
        var pokemon = Legality.AutoLegalize(pkmn!);
        Assert.That(pokemon, Is.Not.Null);
    }
}