using System.Text.Json;
using coreconsole.Models;
using PKHeX.Core;

namespace coreconsole.utils;

public class Helpers
{
    public static JsonSerializerOptions? SerializerOptions;
    // Used for tests
    public static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();
    }

    public static void Init()
    {
        EncounterEvent.RefreshMGDB(string.Empty);
        RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons);
        Sprites.Init();
        
        SerializerOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    public static PKM? PokemonFromBase64(string pokemon, EntityContext context = EntityContext.None)
    {
        try
        {
            var pkmnStrBytes = Convert.FromBase64String(pokemon);

            var pkmn = EntityFormat.GetFromBytes(pkmnStrBytes, context);

            if (pkmn is null)
            {
                Console.Error.WriteLine("base64 is not a pokemon");
                return null;
            }

            return pkmn;
        }
        catch (Exception e) when (e is FormatException or ArgumentNullException)
        {
            Console.Error.WriteLine("invalid base64 string provided");
            return null;
        }
    }
}