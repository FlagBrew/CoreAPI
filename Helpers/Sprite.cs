using Newtonsoft.Json.Linq;
using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreAPI.Helpers
{
    public class Sprite
    {
        public static JObject spriteJSON;

        public static void loadJSON()
        {
            spriteJSON = JObject.Parse(File.ReadAllText("./data/pokemon.json"));

            // getFormNameForJSON("025", 6, 25);
        }

        public static string getFormURL(int species, string generation, string Form, bool shiny, string gender, string SpeciesName)
        {
            string speciesNumText = species.ToString();
            if (speciesNumText.Length == 2)
            {
                speciesNumText = "0" + species.ToString();
            }

            var url = "https://sprites.fm1337.com/pokemon-gen" + (generation != "8" ? "7x/" : "8/") + (shiny ? "shiny/" : "regular/");

            string jsonKey = getFormNameForJSON(speciesNumText, generation, Form);


            var hasFemale = spriteJSON[speciesNumText][generation != "8" ? "gen-7" : "gen-8"]["forms"][jsonKey]["has_female"];
            if (hasFemale != null && (bool) hasFemale && gender == "F")
            {
                url += "female/";
            }


            url += SpeciesName.ToLower();
            if (jsonKey != "$")
            {
                url += jsonKey;
            }

            url += ".png";
            return url;
        }


        private static string getFormNameForJSON(string species, string generation, string Form)
        {
            JToken forms;
            if (Form == "") {
                return "$";
            }

            if (generation == "8")
            {
                forms = spriteJSON[species]["gen-8"];
            } else
            {
                forms = spriteJSON[species]["gen-7"];
            }
            foreach (JProperty sn in forms["forms"])
            {
                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                var formName = rgx.Replace(Form, "").ToLower().Replace(" ", "");
                if (sn.Name.Contains(formName))
                {
                    if (forms[sn]["is_alias_of"] != null)
                    {
                        return (String)forms[species][forms[sn]["is_alias_of"]];
                    }
                    return sn.Name;
                }
            }
            return "$";
        }
        private static IEnumerable<string> getAllForms(int species)
        {
            string[] pkx_forms_1 = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, 1);
            string[] pkx_forms_2 = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, 2);
            string[] pkx_forms_3 = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, 3);
            string[] pkx_forms_4 = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, 4);
            string[] pkx_forms_5 = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, 5);
            string[] pkx_forms_6 = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, 6);
            string[] pkx_forms_7 = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, 7);
            string[] pkx_forms_8 = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, 8);

            return pkx_forms_1.Union(pkx_forms_2).Union(pkx_forms_3).Union(pkx_forms_4).Union(pkx_forms_5).Union(pkx_forms_6).Union(pkx_forms_7).Union(pkx_forms_8);
        }
    }
}
