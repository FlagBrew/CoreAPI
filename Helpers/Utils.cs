using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CoreAPI.Models;
using PKHeX.Core;
using QRCoder;
using static CoreAPI.Models.Encounter;

namespace CoreAPI.Helpers
{
    public static class Utils
    {
        private static readonly List<string> pkmnWithFemaleForms = new List<string> { "abomasnow", "aipom", "alakazam", "ambipom", "beautifly", "bibarel", "bidoof", "blaziken", "buizel", "butterfree", "cacturne", "camerupt", "combee", "combusken", "croagunk", "dodrio", "doduo", "donphan", "dustox", "finneon", "floatzel", "frillish", "gabite", "garchomp", "gible", "girafarig", "gligar", "gloom", "golbat", "goldeen", "gulpin", "gyarados", "heracross", "hippopotas", "hippowdon", "houndoom", "hypno", "jellicent", "kadabra", "kricketot", "kricketune", "ledian", "ledyba", "ludicolo", "lumineon", "luxio", "luxray", "magikarp", "mamoswine", "medicham", "meditite", "meganium", "milotic", "murkrow", "nidoran", "numel", "nuzleaf", "octillery", "pachirisu", "pikachu", "piloswine", "politoed", "pyroar", "quagsire", "raichu", "raticate", "rattata", "relicanth", "rhydon", "rhyhorn", "rhyperior", "roselia", "roserade", "scizor", "scyther", "seaking", "shiftry", "shinx", "sneasel", "snover", "spinda", "staraptor", "staravia", "starly", "steelix", "sudowoodo", "swalot", "tangrowth", "torchic", "toxicroak", "unfezant", "unown", "ursaring", "venusaur", "vileplume", "weavile", "wobbuffet", "wooper", "xatu", "zubat" };
        private static readonly List<string> pkmnEggGroups = new List<string> { "Monster", "Water 1", "Bug", "Flying", "Field" , "Fairy", "Grass", "Human-Like", "Water 3", "Mineral", "Amorphous", "Water 2", "Ditto", "Dragon", "Undiscovered" };

        private static readonly string[] Splitters = {"| ", " |", " | ", "|"};

        public static string[] SplitQueryString(string queryStr)
        {
            return queryStr.ToLower().Split(Splitters, 0);
        }

        public static bool PokemonExists(string Name)
        {
            return Enum.GetNames(typeof(Species)).Any(s => s.ToLower() == Name);
        }

        public static bool PokemonExistsInGeneration(string generation, int speciesNum)
        {
            switch(generation)
            {
                case "1":
                    if(speciesNum >= 1 && speciesNum <= 151)
                    {
                        return true;
                    }
                    break;
                case "2":
                    if (speciesNum >= 1 && speciesNum <= 251)
                    {
                        return true;
                    }
                    break;
                case "3":
                    if (speciesNum >= 1 && speciesNum <= 386)
                    {
                        return true;
                    }
                    break;
                case "4":
                    if (speciesNum >= 1 && speciesNum <= 493)
                    {
                        return true;
                    }
                    break;
                case "5":
                    if (speciesNum >= 1 && speciesNum <= 649)
                    {
                        return true;
                    }
                    break;
                case "6":
                    if (speciesNum >= 1 && speciesNum <= 721)
                    {
                        return true;
                    }
                    break;
                case "7":
                    if (speciesNum >= 1 && speciesNum <= 809)
                    {
                        return true;
                    }
                    break;
                case "LGPE":
                    if ((speciesNum >= 1 && speciesNum <= 151) || speciesNum == 808 || speciesNum == 809)
                    {
                        return true;
                    }
                    break;
                case "8":
                    if (speciesNum >= 1 && speciesNum <= 896)
                    {
                        return true;
                    }
                    break;
                default:
                    Console.WriteLine("Fucker didn't provide any generation, go fuck yourself");
                    return false;
            }
            return false;
        }

        public static PKM GetPKMwithGen(string generation, byte[] data)
        {
            Console.WriteLine("byte: 1 (index 0) " + data[0]);
            Console.WriteLine("byte: 2 (index 1) " + data[1]);
            Console.WriteLine("byte: 3 (index 2) " + data[2]);
            Console.WriteLine("byte: 4 (index 3) " + data[3]);
            /*
            Console.WriteLine(data[1]);
            if(generation == "1")
            {
                data[0] = data[1];
            }
            */
            return generation switch
            {
                "1" => new PokeList1(data)[0],
                "2" => new PokeList2(data)[0],
                "3" => new PK3(data),
                "4" => new PK4(data),
                "5" => new PK5(data),
                "6" => new PK6(data),
                "7" => new PK7(data),
                "8" => new PK8(data),
                "LGPE" => new PB7(data),
                _ => null,
            };
        }

        public static string GetGeneration(PKM pkm)
        {
            return pkm switch
            {
                PK1 _ => "1",
                PK2 _ => "2",
                PK3 _ => "3",
                PK4 _ => "4",
                PK5 _ => "5",
                PK6 _ => "6",
                PK7 _ => "7",
                PB7 _ => "LGPE",
                PK8 _ => "8",
                _ => ""
            };
        }


        public static GameVersion GetGameVersion(string generation)
        {
            return generation switch
            {
                "1" => GameVersion.RBY,
                "2" => GameVersion.GSC,
                "3" => GameVersion.RSE,
                "4" => GameVersion.DPPt,
                "5" => GameVersion.B2W2,
                "6" => GameVersion.ORAS,
                "7" => GameVersion.USUM,
                "8" => GameVersion.SWSH,
                _ => GameVersion.Any,
            };
        }

        // For fuck's sake rewrite this later, I can't stand looking at it anymore
        public static string GetPokeSprite(int pokemonNum, string pokemonName, string pokemonGender, string form, string generation, bool isShiny)
        {
            var formSet = false;
            switch (pokemonName)
            {
                case "Type: Null":
                    {
                        pokemonName = "type-null";
                        break;
                    }
                case "Farfetch'd":
                case "Farfetch’d":
                    {
                        pokemonName = "farfetchd";
                        break;
                    }
                case "Nidoran♂":
                case "Nidoran♀":
                    {
                        if (pokemonName.Contains("♂"))
                        {
                            pokemonName = "nidoran-m";
                        }
                        else
                        {
                            pokemonName = "nidoran-f";
                        }
                        formSet = true;
                        break;
                    }
                case "Mr. Mime":
                    {
                        pokemonName = "mr-mime";
                        break;
                    }
                case "Mime Jr.":
                    {
                        pokemonName = "mime-jr";
                        break;
                    }
                case "Tapu Koko":
                case "Tapu Lele":
                case "Tapu Bulu":
                case "Tapu Fini":
                    {
                        pokemonName = pokemonName.Replace(" ", "-");
                        break;
                    }
                case "Flabébé":
                    {
                        pokemonName = "flabebe";
                        break;
                    }
                case "Meowstic":
                    {
                        if (generation != "8")
                        {
                            if (pokemonGender == "M")
                            {
                                form = "male";
                            }
                            else
                            {
                                form = "female";
                            }
                        }
                        break;
                    }
                case "Rockruff":
                    {
                        formSet = true;
                        break;
                    }
                case "Genesect":
                    {
                        formSet = true;
                        break;
                    }
                case "Necrozma":
                    {
                        switch (form.ToLower())
                        {
                            case "dawn":
                                {
                                    form = "dawn-wings";
                                    break;
                                }
                            case "dusk":
                                {
                                    form = "dusk-mane";
                                    break;
                                }
                        }
                        break;
                    }
            }
            if (form.ToLower() == "large" && pokemonName.ToLower() != "gourgeist")
            {
                formSet = true;
            }
            pokemonName = pokemonName.Replace("'", "").Replace("é", "e").Replace("’", "").Replace(" ", "-");
            form = form.Replace("%-C", "").Replace("%", "").Replace("é", "e");
            var url = "http://server.charizard-is.best/"; // god fucking dammit I forgot the slash earlier and they're not using HTTPS. Fuck me.
            if (generation == "LGPE")
            {
                generation = "7";
            }
            int result = int.Parse(generation);
            switch (generation)
            {
                case "1":
                    {
                        url += "red-blue/normal/" + pokemonName.ToLower() + "-color";
                        formSet = true;
                        break;
                    }
                case "2":
                    {
                        url += "crystal/";
                        if (isShiny)
                        {
                            url += "shiny/" + pokemonName.ToLower();
                        }
                        else
                        {
                            url += "normal/" + pokemonName.ToLower();
                        }
                        break;
                    }
                case "3":
                    {
                        url += "emerald/";
                        if (isShiny)
                        {
                            url += "shiny/" + pokemonName.ToLower();
                        }
                        else
                        {
                            url += "normal/" + pokemonName.ToLower();
                        }
                        break;
                    }
                case "4":
                    {
                        url += "heartgold-soulsilver/";
                        if (isShiny)
                        {
                            url += "shiny/" + pokemonName.ToLower();
                        }
                        else
                        {
                            url += "normal/" + pokemonName.ToLower();
                        }
                        break;
                    }
                case "5":
                    {
                        url += "black-white-2/";
                        if (isShiny)
                        {
                            url += "shiny/" + pokemonName.ToLower();
                        }
                        else
                        {
                            url += "normal/" + pokemonName.ToLower();
                        }
                        break;
                    }
                case "6":
                    {
                        url += "omega-ruby-alpha-sapphire/";
                        if (isShiny)
                        {
                            url += "shiny/" + pokemonName.ToLower();
                        }
                        else
                        {
                            url += "normal/" + pokemonName.ToLower();
                        }
                        break;
                    }
                case "7":
                    {
                        url += "ultra-sun-ultra-moon/";
                        if (isShiny)
                        {
                            url += "shiny/" + pokemonName.ToLower();
                        }
                        else
                        {
                            url += "normal/" + pokemonName.ToLower();
                        }
                        break;
                    }
                case "8":
                    {
                        url += "sprites/8/";
                        if(pokemonGender == "female")
                        {
                            url += "female/";
                        }
                        if (isShiny)
                        {
                            url += "shiny/" + pokemonName.ToLower();
                        }
                        else
                        {
                            url += "normal/" + pokemonName.ToLower();
                        }
                        break;
                    }
            }
            switch (pokemonNum)
            {
                case 774:
                    {
                        if (form.StartsWith("M"))
                        {
                            form = "meteor";
                        }
                        else
                        {
                            form = form.Replace("C-", "").ToLower() + "-core";
                        }
                        url += "-" + form;
                        formSet = true;
                        break;
                    }
                case 201:
                    {
                        if (form == "!")
                        {
                            form = "em";
                        }
                        else if (form == "?")
                        {
                            form = "qm";
                        }
                        else
                        {
                            form = form.ToLower();
                        }
                        url += "-" + form;
                        formSet = true;
                        break;
                    }
                case 386:
                case 493:
                case 479:
                case 646:
                case 550:
                    {
                        if ((result == 5 && pokemonName.ToLower() == "kyurem") || (result < 5 && pokemonName.ToLower() == "rotom"))
                        {
                            formSet = true;
                        }
                        else if (result == 5 && pokemonName.ToLower() == "basculin")
                        {
                            url += "-" + form.ToLower() + "-striped";
                            formSet = true;
                        }
                        else
                        {
                            url += "-" + form.ToLower();
                            formSet = true;
                        }
                        break;
                    }
                case 25:
                    {
                        if (form.ToLower() != "normal")
                        {
                            if (result == 6)
                            {
                                url += "-cosplay";
                                formSet = true;
                            }
                            else if (result == 7)
                            {
                                if (form.ToLower() != "cosplay")
                                {
                                    url += "-" + form.ToLower() + "-cap";
                                    formSet = true;
                                }
                                else
                                {
                                    url += "-cosplay";
                                    formSet = true;
                                }
                            }
                        }
                        break;
                    }
                case 676:
                    {
                        if (form.ToLower() == "natural")
                        {
                            formSet = true;
                        }
                        break;
                    }
                case 664:
                case 665:
                case 658:
                    {
                        formSet = true;
                        break;
                    }
                case 414:
                    {
                        formSet = true;
                        break;
                    }
                case 741:
                    {
                        if (form.ToLower() == "pa’u")
                        {
                            form = "pau";
                        }
                        break;
                    }
                case 778:
                    {
                        if (form.ToLower() == "disguised")
                        {
                            formSet = true;
                        }

                        break;
                    }
            }

            if (!formSet && result > 3)
            {
                if (form.ToLower() != "normal" && form != "" && form != "♀")
                {
                    if (form.ToLower() == "alola")
                    {
                        form = "alolan";
                    }
                    url += "-" + form.Replace(" ", "-").ToLower();
                    formSet = true;
                }
            }
            if (pkmnWithFemaleForms.Any(p => p == pokemonName.ToLower()) && pokemonGender == "F" && result > 3 && !formSet)
            {
                if (form.ToLower() == "normal" && form.Length == 0)
                {
                    url += "-f";
                    formSet = true;
                }
            }

            url += ".png";
            // Console.WriteLine(origin_game);
            // Console.WriteLine(form);
            // Console.WriteLine(generation);
            return url;
        }
        public static string GetForm(PKM pkm, int alt)
        {
            var ds = FormConverter.GetFormList(pkm.Species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, pkm.Format);
            if (ds.Length > 1)
            {
                return ds[alt];
            }

            return ds[0];
        }
        public static string GenerateQR(string data)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.L);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(3);
            qrCode.Dispose();
            qrGenerator.Dispose();
            return Convert.ToBase64String(qrCodeAsPngByteArr);
        }

        public static GameVersion GetGameVersion(PKM pkm)
        {
            return pkm switch
            {
                PK1 _ => GameVersion.YW,
                PK2 _ => GameVersion.C,
                PK3 _ => GameVersion.E,
                PK4 _ => GameVersion.HG,
                PK5 _ => GameVersion.B2,
                PK6 _ => GameVersion.OR,
                PK7 _ => GameVersion.UM,
                PB7 _ => GameVersion.GE,
                PK8 _ => GameVersion.SW,
                _ => 0
            };
        }

        public static string GetStringFromRegex(string regexpattern, string data)
        {
            Regex r = new Regex(regexpattern);
            var match = r.Match(data);
            if (match.Success)
                return match.Value;
            return "";
        }
        public static List<GenerationLocation> GetLocations(string pokemon, string generation, string[] moves)
        {
            // If the pokemon doesn't exist, then we return a null list and handle it on the controller
            if (!Enum.GetNames(typeof(Species)).Any(s => s.ToLower() == pokemon))
            {
                return null;
            }
            var encounters = EncounterLearn.GetLearnSummary(pokemon, moves);
            var encountertype = "";
            var genlocs = new List<GenerationLocation>();
            var locations = new List<Location>();
            foreach (var encounter in encounters)
            {
                //Console.WriteLine(encounter);
                // Lines start start with Equal signs define the encounter type
                if (encounter.StartsWith("="))
                {
                    // Check to see if the Locations list isn't empty
                    if (locations.Count > 0)
                    {
                        // Add the GenerationLocation entry
                        genlocs.Add(new GenerationLocation
                        {
                            EncounterType = encountertype,
                            Locations = locations
                        });
                    }
                    // create a new locations list
                    locations = new List<Location>();
                    // set the encountertype and then next loop
                    encountertype = encounter.Replace("=", "");
                    continue;
                }
                // get the generation from the encounter line
                var gen = GetStringFromRegex("Gen[1-9]", encounter);
                // Check to see if the generation is the one we want
                if (!gen.Contains(generation))
                {
                    // if it doesn't, next loop
                    continue;
                }
                // Now we get the location
                var loc = GetStringFromRegex("(?<=.{8}).+?(?=:)", encounter);
                // And the games for the location
                var games = GetStringFromRegex(@"([\t ][A-Z |,]{1,100}$|Any)", encounter);
                // We also have to do some cleanup on the games data
                games = games.Replace(" ", "");
                games = games.Trim(':');
                games = games.Trim('\t');
                // Now we need to split the games list into an array
                var gamesArray = games.Split(',');
                // Next we add the data to the location list
                locations.Add(new Location
                {
                    Name = loc,
                    Games = gamesArray,
                });
            }
            // Add the last entry provided the location count isn't 0.
            if (locations.Count > 0)
            {
                // Add the GenerationLocation entry
                genlocs.Add(new GenerationLocation
                {
                    EncounterType = encountertype,
                    Locations = locations
                });
            }
            return genlocs;
        }

        public enum PokeColor
        {
            Red,
            Blue,
            Yellow,
            Green,
            Black,
            Brown,
            Purple,
            Gray,
            White,
            Pink,
        }

        public static string[] GetFormList(in int species)
        {
            var s = GameInfo.Strings;
            if (species == (int)Species.Alcremie)
                return FormConverter.GetAlcremieFormList(s.forms);
            return FormConverter.GetFormList(species, s.Types, s.forms, GameInfo.GenderSymbolASCII, 8).ToArray();
        }

        public static BasePokemon GetBasePokemon(int species, int form)
        {
            try
            {
                var gameStrings = GameInfo.Strings;
                var pi = PersonalTable.SWSH.GetFormeEntry(species, form);
                if (pi.HP == 0)
                    pi = PersonalTable.USUM.GetFormeEntry(species, form);

                var abilities = new List<string>();
                var types = new List<string>();
                var groups = new List<string>();
                foreach (var a in pi.Abilities)
                {
                    abilities.Add(gameStrings.abilitylist[a]);
                }
                foreach (var t in pi.Types)
                {
                    types.Add(gameStrings.types[t]);
                }
                foreach (var e in pi.EggGroups)
                {
                    groups.Add(pkmnEggGroups[e - 1]);
                }
                var bp = new BasePokemon
                {
                    HP = pi.HP,
                    ATK = pi.ATK,
                    DEF = pi.DEF,
                    SPE = pi.SPE,
                    SPA = pi.SPA,
                    SPD = pi.SPD,
                    CatchRate = pi.CatchRate,
                    EvoStage = pi.EvoStage,
                    Gender = pi.Gender,
                    HatchCycles = pi.HatchCycles,
                    BaseFriendship = pi.BaseFriendship,
                    EXPGrowth = pi.EXPGrowth,
                    Ability1 = "N/A",
                    Ability2 = "N/A",
                    AbilityH = "N/A",
                    Color = Enum.GetNames(typeof(PokeColor))[pi.Color],
                    Height = pi.Height,
                    Weight = pi.Weight,
                    HasHiddenAbility = false,
                    Types = types,
                    EggGroups = groups,
                    IsDualGender = pi.IsDualGender,
                    Genderless = pi.Genderless,
                    OnlyFemale = pi.OnlyFemale,
                    OnlyMale = pi.OnlyMale,
                    BST = pi.BST
                };
                switch (abilities.Count)
                {
                    case 1:
                        bp.Ability1 = abilities[0];
                        bp.Ability2 = abilities[0];
                        break;
                    case 2:
                        bp.Ability1 = abilities[0];
                        bp.Ability2 = abilities[1];
                        break;
                    case 3:
                        bp.Ability1 = abilities[0];
                        bp.Ability2 = abilities[1];
                        bp.AbilityH = abilities[2];
                        bp.HasHiddenAbility = true;
                        break;
                }
                return bp;
            }
            catch
            {
                return null;
            }
        }
    }
}
