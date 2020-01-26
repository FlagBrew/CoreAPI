using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PKHeX.Core;
using QRCoder;
using static CoreAPI.Models.Encounter;

namespace CoreAPI.Helpers
{
    public class Utils
    {
        private static readonly List<string> pkmnWithFemaleForms = new List<string> { "abomasnow", "aipom", "alakazam", "ambipom", "beautifly", "bibarel", "bidoof", "blaziken", "buizel", "butterfree", "cacturne", "camerupt", "combee", "combusken", "croagunk", "dodrio", "doduo", "donphan", "dustox", "finneon", "floatzel", "frillish", "gabite", "garchomp", "gible", "girafarig", "gligar", "gloom", "golbat", "goldeen", "gulpin", "gyarados", "heracross", "hippopotas", "hippowdon", "houndoom", "hypno", "jellicent", "kadabra", "kricketot", "kricketune", "ledian", "ledyba", "ludicolo", "lumineon", "luxio", "luxray", "magikarp", "mamoswine", "medicham", "meditite", "meganium", "milotic", "murkrow", "nidoran", "numel", "nuzleaf", "octillery", "pachirisu", "pikachu", "piloswine", "politoed", "pyroar", "quagsire", "raichu", "raticate", "rattata", "relicanth", "rhydon", "rhyhorn", "rhyperior", "roselia", "roserade", "scizor", "scyther", "seaking", "shiftry", "shinx", "sneasel", "snover", "spinda", "staraptor", "staravia", "starly", "steelix", "sudowoodo", "swalot", "tangrowth", "torchic", "toxicroak", "unfezant", "unown", "ursaring", "venusaur", "vileplume", "weavile", "wobbuffet", "wooper", "xatu", "zubat" };
        public static string FixQueryString(string queryStr)
        {
            queryStr = queryStr.Replace("| ", "|").Replace(" | ", "|").Replace(" |", "|").ToLower();
            return queryStr;
        }

        public static string[] SplitQueryString(string queryStr)
        {
            var queries = queryStr.Split('|');
            return queries;
        }

        public static bool PokemonExists(string Name)
        {
            if (!Enum.GetNames(typeof(Species)).Any(s => s.ToLower() == Name))
            {
                return false;
            }
            return true;
        }

        public static bool MoveExists(string Name)
        {
            if (!Util.GetMovesList(GameLanguage.DefaultLanguage).Any(m => m.ToLower().Contains(Name)))
            {
                return false;
            }
             return true;
        }

        public static string GetGeneration(PKM pkm)
        {
            var Generation = "";
            if (pkm.GetType() == typeof(PK4))
            {
                Generation = "4";
            }
            else if (pkm.GetType() == typeof(PK5))
            {
                Generation = "5";

            }
            else if (pkm.GetType() == typeof(PK6))
            {
                Generation = "6";
            }
            else if (pkm.GetType() == typeof(PK7))
            {
                Generation = "7";
            }
            else if (pkm.GetType() == typeof(PB7))
            {
                Generation = "LGPE";
            }
            else if (pkm.GetType() == typeof(PK8))
            {
                Generation = "8";
            }
            else if (pkm.GetType() == typeof(PK3))
            {
                Generation = "3";
            }
            else if (pkm.GetType() == typeof(PK2))
            {
                Generation = "2";
            }
            else if (pkm.GetType() == typeof(PK1))
            {
                Generation = "1";
            }
            return Generation;
        }
        // For fuck's sake rewrite this later, I can't stand looking at it anymore
        public static string GetPokeSprite(int pokemonNum, string pokemonName, string pokemonGender, string form, string generation, bool isShiny)
        {
            if (pokemonNum > 807 || generation == "8")
            {
                return "https://flagbrew.org/static/img/blank.png";
            }
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
                        if (pokemonGender == "M")
                        {
                            form = "male";
                        }
                        else
                        {
                            form = "female";
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
            pokemonName = pokemonName.Replace("'", "").Replace("é", "e").Replace("’", "");
            form = form.Replace("%-C", "").Replace("%", "").Replace("é", "e");
            var url = "https://sprites.fm1337.com/";
            if (generation == "LGPE")
            {
                generation = "7";
            }
            int result = Int32.Parse(generation);
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
                        if (result == 5 && pokemonName.ToLower() == "kyurem" || result < 5 && pokemonName.ToLower() == "rotom")
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
                if (form.ToLower() == "normal" && form == "")
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
            if (ds.Count() > 1)
            {
                return ds[alt];
            }
            else
            {
                return ds[0];
            }
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
            GameVersion version = 0;
            if (pkm.GetType() == typeof(PK4))
            {
                version = GameVersion.HG;
            }
            else if (pkm.GetType() == typeof(PK5))
            {
                version = GameVersion.B2;

            }
            else if (pkm.GetType() == typeof(PK6))
            {
                version = GameVersion.OR;
            }
            else if (pkm.GetType() == typeof(PK7))
            {
                version = GameVersion.UM;
            }
            else if (pkm.GetType() == typeof(PB7))
            {
                version = GameVersion.GE;
            }
            else if (pkm.GetType() == typeof(PK8))
            {
                version = GameVersion.SW;
            }
            else if (pkm.GetType() == typeof(PK3))
            {
                version = GameVersion.E;
            }
            else if (pkm.GetType() == typeof(PK2))
            {
                version = GameVersion.C;
            }
            else if (pkm.GetType() == typeof(PK1))
            {
                version = GameVersion.YW;
            }
            return version;
        }

        public static string GetStringFromRegex(string regexpattern, string data)
        {
            Regex r = new Regex(regexpattern);
            if (r.Match(data).Success)
            {
                return r.Match(data).Value;
            }
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
            var firstrun = true;
            var locations = new List<Location>();
            foreach (var encounter in encounters)
            {
                // Lines start start with Equal signs define the encounter type
                if (encounter.StartsWith("="))
                {
                    if (!firstrun)
                    {
                        // Check to see if the Locations list isn't empty
                        if(locations.Count > 0)
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
                    } else
                    {
                        firstrun = false; 
                    }
                    // set the encountertype and then next loop
                    encountertype = encounter.Replace("=", "");
                    continue;
                }
                // get the generation from the encounter line
                var gen = GetStringFromRegex(@"Gen[0-9]", encounter);
                // Check to see if the generation is the one we want
                if (!gen.Contains(generation))
                {
                    // if it doesn't, next loop
                    continue;
                }
                // Now we get the location
                var loc = GetStringFromRegex(@"(?<=.{8}).+?(?=:)", encounter);
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
            return genlocs;
        }
    }
}
