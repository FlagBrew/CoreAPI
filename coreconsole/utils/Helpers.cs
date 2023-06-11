using System.Text.Json;
using coreconsole.Models;
using PKHeX.Core;
using Sentry;

namespace coreconsole.utils;

public static class Helpers
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

    public static PKM PokemonFromBase64(string pokemon, EntityContext context = EntityContext.None)
    {
        try
        {
            var pkmnStrBytes = Convert.FromBase64String(pokemon);

            var pkmn = EntityFormat.GetFromBytes(pkmnStrBytes, context);

            if (pkmn is not null) return pkmn;
            Console.Error.WriteLine("{\"error\": \"base64 is not a pokemon\"}");
            SentrySdk.CaptureMessage(level: SentryLevel.Error, message: "base64 provided is not a pokemon");
            Environment.Exit((int)enums.ExitCode.Base64NotPokemon);
        }
        catch (Exception e) when (e is FormatException or ArgumentNullException)
        {
            Console.Error.WriteLine("{\"error\": \"invalid base64 string provided\"}");
            SentrySdk.CaptureException(e);
            Environment.Exit((int)enums.ExitCode.BadBase64);
        }
        catch (Exception e) when (e is not FormatException and not ArgumentNullException)
        {
            SentrySdk.CaptureException(e);
            Environment.Exit((int)enums.ExitCode.UnknownErrorDuringBase64ToPokemon);
        }

        return null;
    }

    public static bool LoadEnv()
    {
        if (!File.Exists(".env"))
        {
            Console.Error.WriteLine(".env is missing");
            return false;
        }

        foreach (var line in File.ReadAllLines(".env"))
        {
            var parts = line.Split("=");
            if (parts.Length != 2) continue;

            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }

        return true;
    }
}