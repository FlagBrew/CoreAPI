using System.Text.Json;
using System.Text.Json.Serialization;
using PKHeX.Core;
using Sentry;

namespace coreconsole.Models;

public struct Sprites
{
    private const string BaseUrl = "https://cdn.sigkill.tech/sprites/";

    private static readonly List<string> SpeciesWithFemaleForms = new()
    {
        "frillish",
        "hippopotas",
        "hippowdon",
        "jellicent",
        "meowstic",
        "pikachu",
        "pyroar",
        "unfezant",
        "wobbuffet",
        "basculegion",
        "indeedee"
    };

    private static readonly List<string> AlcremieDecorations = new()
    {
        "strawberry",
        "berry",
        "love",
        "star",
        "clover",
        "flower",
        "ribbon"
    };

    private static readonly Dictionary<string, string> ReplaceChars = new()
    {
        { "♀", "f" },
        { "♂", "m" },
        { "é", "e" },
        { "’", "" },
        { "'", "" },
        { ": ", "-" },
        { " ", "-" },
        { ".", "" }
    };

    public static JsonElement SpeciesBindings;
    private static JsonElement _itemBindings;
    private static bool _spritesLoaded;

    [JsonPropertyName("species")] public string Species { get; set; }
    [JsonPropertyName("item")] public string Item { get; set; }

    public static void Init()
    {
        try
        {
            var speciesBindingsFile = File.ReadAllText("data/bindings.json");
            SpeciesBindings = JsonSerializer.Deserialize<JsonElement>(speciesBindingsFile);
            var itemBindingsFile = File.ReadAllText("data/item-map.json");
            _itemBindings = JsonSerializer.Deserialize<JsonElement>(itemBindingsFile);
            _spritesLoaded = true;
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            _spritesLoaded = false;
        }
    }


    public Sprites(EntitySummary summary, PKM pkmn)
    {
        if (!_spritesLoaded)
        {
            Species = "";
            Item = "";
            return;
        }

        Species = ConstructSpeciesSprite(summary, pkmn);
        Item = ConstructItemSprite(pkmn.HeldItem);
    }

    private string ConstructSpeciesSprite(EntitySummary summary, PKM pkmn)
    {
        var checkBinding = true;
        var species = summary.Species.ToLower();
        string[] forms;
        try
        {
            forms = FormConverter.GetFormList(pkmn.Species, GameInfo.Strings.types, GameInfo.Strings.forms,
                GameInfo.GenderSymbolASCII, pkmn.Context);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            return "";
        }

        var path = $"{BaseUrl}";

        if (pkmn.Generation <= 7)
            path += "pokemon-gen7x/";
        else
            path += "pokemon-gen8/";

        path += pkmn.IsShiny ? "shiny/" : "regular/";
        string form;
        try
        {
            form = (forms[pkmn.Form] ?? "").ToLower();
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            form = "";
        }

        if (SpeciesWithFemaleForms.Contains(species) && pkmn.Gender == 2)
        {
            path += "female/";

            if (species == "pikachu" && form == "normal") checkBinding = false;
        }

        if (form == "") checkBinding = false;

        if (checkBinding)
        {
            if (SpeciesBindings.TryGetProperty($"{species.Replace(" ", "_")}_{form.Replace(" ", "_")}",
                    out var binding))
            {
                if (species == "alcremie" && pkmn is IFormArgument ifo)
                {
                    try
                    {
                        if (!Enum.TryParse(ifo.FormArgument.ToString(), out AlcremieDecoration dec)) return "";
                        var name = Enum.GetName(dec)!.ToLower();
                        if (!binding.TryGetProperty("file", out var file)) return "";
                        path += $"{file.GetString()!.Replace(".png", $"-{name}.png")}";
                    }
                    catch (Exception e)
                    {
                        SentrySdk.CaptureException(e);
                    }
                }
                else
                {
                    path += $"{binding.GetProperty("file").GetString()}";
                }

                return path;
            }

            ;
            return $"{path}{species}.png";
        }

        species = ReplaceChars.Aggregate(species, (current, replace) => current.Replace(replace.Key, replace.Value));


        return $"{path}{species}.png";
    }

    private string ConstructItemSprite(int item)
    {
        if (item == 0) return "";

        if (!_itemBindings.TryGetProperty($"item_{item.ToString().PadLeft(4, '0')}", out var path)) return "";
        ;

        return $"{BaseUrl}items/{path.GetString()}.png";
    }
}