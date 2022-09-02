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
        private static readonly List<string> pkmnWithFemaleForms = new List<string> { "abomasnow", "aipom", "alakazam", "ambipom", "beautifly", "bibarel", "bidoof", "blaziken", "buizel", "butterfree", "cacturne", "camerupt", "combee", "combusken", "croagunk", "dodrio", "doduo", "donphan", "dustox", "finneon", "floatzel", "frillish", "gabite", "garchomp", "gible", "girafarig", "gligar", "gloom", "golbat", "goldeen", "gulpin", "gyarados", "heracross", "hippopotas", "hippowdon", "houndoom", "hypno", "jellicent", "kadabra", "kricketot", "kricketune", "ledian", "ledyba", "ludicolo", "lumineon", "luxio", "luxray", "magikarp", "mamoswine", "medicham", "meditite", "meganium", "milotic", "murkrow", "nidoran", "numel", "nuzleaf", "octillery", "pachirisu", "pikachu", "piloswine", "politoed", "pyroar", "quagsire", "raichu", "raticate", "rattata", "relicanth", "rhydon", "rhyhorn", "rhyperior", "roselia", "roserade", "scizor", "scyther", "seaking", "shiftry", "shinx", "sneasel", "snover", "spinda", "staraptor", "staravia", "starly", "steelix", "sudowoodo", "swalot", "tangrowth", "torchic", "toxicroak", "unfezant", "unown", "ursaring", "venusaur", "vileplume", "weavile", "wobbuffet", "wooper", "xatu", "zubat", "meowstic", "indeedee", "basculegion" };
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
                case "BDSP":
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
                case "PLA":
                    // Gotta build the list manually >_< (and yes I manually typed out all these, it sucked)
                    var supported = new List<int>(){ 722, 723, 724, 155, 156, 157, 501, 502, 503, 399, 400, 396, 397, 398, 403, 404, 405, 265, 266, 267, 268, 269, 77, 78, 133, 134, 135, 136, 196, 197, 470, 471, 700, 41, 42, 169, 425, 426, 401, 402, 418, 419, 412, 413, 414, 74, 75, 76, 234, 899, 446, 143, 46, 47, 172, 25, 26, 63, 64, 65, 390, 391, 392, 427, 428, 420, 421, 54, 55, 415, 416, 123, 900, 212, 214, 439, 122, 190, 424, 129, 130, 422, 423, 211, 904, 440, 113, 242, 406, 315, 407, 455, 548, 549, 114, 465, 339, 340, 453, 454, 280, 281, 282, 475, 193, 469, 449, 450, 417, 434, 435, 216, 217, 901, 704, 705, 706, 95, 208, 111, 112, 464, 438, 185, 108, 463, 175, 176, 468, 387, 388, 389, 137, 233, 474, 092, 093, 094, 442, 198, 430, 201, 363, 364, 365, 223, 224, 451, 452, 58, 59, 431, 432, 66, 67, 68, 441, 355, 356, 477, 393, 394, 395, 458, 226, 550, 902, 37, 38, 72, 73, 456, 457, 240, 126, 467, 81, 82, 462, 436, 437, 239, 125, 466, 207, 472, 443, 444, 445, 299, 476, 100, 101, 479, 433, 358, 200, 429, 173, 35, 36, 215, 903, 461, 361, 362, 478, 408, 409, 410, 411, 220, 221, 473, 712, 713, 459, 570, 571, 672, 628, 447, 448, 480, 481, 482, 485, 486, 488, 641, 642, 645, 905, 483, 484, 487, 493, 489, 490, 492, 491};
                    if (supported.FirstOrDefault(sn => sn == speciesNum) != 0)
                    {
                        return true;
                    }
                    break;
                default:
                    Console.WriteLine("Generation wasn't provided. We determine and thus must return false");
                    return false;
            }
            return false;
        }

        public static PKM GetPKMwithGen(string generation, byte[] data)
        {
/*            Console.WriteLine("byte: 1 (index 0) " + data[0]);
            Console.WriteLine("byte: 2 (index 1) " + data[1]);
            Console.WriteLine("byte: 3 (index 2) " + data[2]);
            Console.WriteLine("byte: 4 (index 3) " + data[3]);*/
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
                "BDSP" => new PB8(data),
                "PLA" => new PA8(data),
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
                PB8 _ => "BDSP",
                PA8 _ => "PLA",
                _ => ""
            };
        }


        public static GameVersion GetGameVersion(string generation)
        {
            // Also supports game code
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
                "BDSP" => GameVersion.BDSP,
                "PLA" => GameVersion.PLA,
                "LGPE" => GameVersion.GG,
                _ => GameVersion.Any,
            };
        }

        public static string GetGenerationFromVersion(int version)
        {
            int[] gen1 = { 35, 36, 37, 38, 50, 51, 84, 83 };
            int[] gen2 = { 39, 40, 41, 52, 53, 85 };
            int[] gen3 = { 1, 2, 3, 4, 5, 54, 55, 56, 57, 58, 59 };
            int[] gen4 = { 10, 11, 12, 7, 8, 60, 61, 62, 0x3F };
            int[] gen5 = { 20, 21, 22, 23, 0x40, 65 };
            int[] gen6 = { 24, 25, 26, 27, 66, 67, 68 };
            int[] gen7 = { 30, 0x1F, 0x20, 33, 69, 70 };
            int[] genLGPE = { 71, 34, 42, 43 };
            int[] gen8 = { 44, 45, 47, 72 };
            int[] genBDSP = { 73, 48, 49 };
            switch (true)
            {
                case var _ when gen1.Contains(version):
                    return "1";
                case var _ when gen2.Contains(version):
                    return "2";
                case var _ when gen3.Contains(version):
                    return "3";
                case var _ when gen4.Contains(version):
                    return "4";
                case var _ when gen5.Contains(version):
                    return "5";
                case var _ when gen6.Contains(version):
                    return "6";
                case var _ when gen7.Contains(version):
                    return "7";
                case var _ when genLGPE.Contains(version):
                    return "LGPE";
                case var _ when gen8.Contains(version):
                    return "8";
                case var _ when genBDSP.Contains(version):
                    return "BDSP";
                case var _ when version == 471:
                    return "PLA";
                default:
                    throw new NotSupportedException();
            }
        }

        public static string GetForm(PKM pkm, int alt)
        {
            var ds = FormConverter.GetFormList(pkm.Species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, pkm.Context);
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
                PB8 _ => GameVersion.BD,
                PA8 _ => GameVersion.PLA,
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
        public static List<GenerationLocation> GetLocations(string pokemon, string generation, string specialGen, string[] moves)
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
                var games = GetStringFromRegex(@"([\t ][A-Z , a-z 0-9]{1,100}$|Any)", encounter);
                // We also have to do some cleanup on the games data
                games = games.Replace(" ", "");
                games = games.Trim(':');
                games = games.Trim('\t');
                if (specialGen == "" && ContainsAny(games, "BD", "SP", "PLA", "GG", "GE", "GO", "GP")) {
                    continue;
                }

                if (specialGen == "BDSP" && !ContainsAny(games, "BD", "SP")) {
                    continue;
                }

                if (specialGen == "PLA" && !games.Contains("PLA"))
                {
                    continue;
                }

                if (specialGen == "LGPE" && !ContainsAny(games, "GO", "GG", "GP", "GE"))
                {
                    continue;
                }

                if (specialGen != "" && games.Contains("Gen"))
                {
                    // Remove the gen out of there as we only want our special generation and not just a generic one.
                    games = Regex.Replace(games, @",Gen[0-9]{1,2}", "");
                }

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

        public static string[] GetFormList(in int species, in string generation = "8")
        {
            var s = GameInfo.Strings;
            if (species == (int)Species.Alcremie)
                return FormConverter.GetAlcremieFormList(s.forms);
            return FormConverter.GetFormList((ushort)species, s.Types, s.forms, GameInfo.GenderSymbolASCII, GetEntityFromGeneration(generation)).ToArray();
        }

        public static BasePokemon GetBasePokemon(int species, byte form, string generation)
        {
            try
            {
                dynamic pi;
                var gameStrings = GameInfo.Strings;
                pi = PersonalTable.SWSH.GetFormEntry((ushort)species, form);
                if (pi.HP == 0)
                    pi = PersonalTable.USUM.GetFormEntry((ushort)species, form);
                   

                var abilities = new List<string>();
                var types = new List<string>();
                var groups = new List<string>();
                foreach (var a in pi.Abilities)
                {
                    abilities.Add(gameStrings.abilitylist[a]);
                }
                types.Add(gameStrings.types[pi.Type1]);
                if (pi.Type2 != -1)
                {
                    types.Add(gameStrings.types[pi.Type2]);
                }
                if (pi.EggGroup1 != -1)
                {
                    groups.Add(pkmnEggGroups[pi.EggGroup1 - 1]);
                }
                if (pi.EggGroup2 != -1)
                {
                    groups.Add(pkmnEggGroups[pi.EggGroup2 - 1]);
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
                    BST = pi.ATK + pi.DEF + pi.SPE + pi.SPA + pi.SPD + pi.HP,
                    SpeciesSpriteURL = Sprite.getFormURL(species, generation.ToString(), gameStrings.forms[form], false, (pi.Gender == 1 ? "F" : pi.Gender == 0 ? "M": "-"), gameStrings.Species[species]),
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        public static EntityContext GetEntityFromGeneration(string generation)
        {
            switch (generation)
            {
                case "1":
                    return EntityContext.Gen1;
                case "2":
                    return EntityContext.Gen2;
                case "3":
                    return EntityContext.Gen3;
                case "4":
                    return EntityContext.Gen4;
                case "5":
                    return EntityContext.Gen5;
                case "6":
                    return EntityContext.Gen6;
                case "7":
                    return EntityContext.Gen7;
                case "8":
                    return EntityContext.Gen8;
                case "LGPE":
                    return EntityContext.Gen7b;
                case "BDSP":
                    return EntityContext.Gen8a;
                case "PLA":
                    return EntityContext.Gen8b;
            }
            throw new Exception("Unsupported generation");
        }
        // Borrowed from https://stackoverflow.com/questions/3519539/how-to-check-if-a-string-contains-any-of-some-strings
        public static bool ContainsAny(this string haystack, params string[] needles)
        {
            foreach (string needle in needles)
            {
                if (haystack.Contains(needle))
                    return true;
            }

            return false;
        }
    }
}
