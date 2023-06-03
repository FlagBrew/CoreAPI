using System.Text.Json;
using PKHeX.Core;

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

    public string Species { get; set; }
    public string Item { get; set; }

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
            Console.WriteLine("Failed to load sprites!");
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
        var forms = FormConverter.GetFormList(pkmn.Species, GameInfo.Strings.types, GameInfo.Strings.forms,
            GameInfo.GenderSymbolASCII, pkmn.Context);
        
        var path = $"{BaseUrl}";

        if (pkmn.Generation <= 7)
        {
            path += "pokemon-gen7x/";
        }
        else
        {
            path += "pokemon-gen8/";
        }

        path += pkmn.IsShiny ? "shiny/" : "regular/";
        var form = (forms[pkmn.Form] ?? "").ToLower();
        
        if (SpeciesWithFemaleForms.Contains(species) && pkmn.Gender == 2)
        {
            path += "female/";

            if (species == "pikachu" && form == "normal")
            {
                checkBinding = false;
            }
        }

        if (form == "")
        {
            checkBinding = false;
        }

        if (checkBinding)
        {
            if (SpeciesBindings.TryGetProperty($"{species.Replace(" ", "_")}_{form.Replace(" ", "_")}",
                    out JsonElement binding))
            {
                if (species == "alcremie" && pkmn is IFormArgument ifo)
                {
                    if (!Enum.TryParse(ifo.FormArgument.ToString(), out AlcremieDecoration dec))
                    {
                        return "";
                    }
                    
                    var name = Enum.GetName(dec)!.ToLower();
                    if (!binding.TryGetProperty("file", out JsonElement file))
                    {
                        return "";
                    }
                    path += $"{file.GetString()!.Replace(".png", $"-{name}.png")}";
                } else {
                    path += $"{binding.GetProperty("file").GetString()}";
                }

                return path;
            };
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