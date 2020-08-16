
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Helpers;
using System.ComponentModel.DataAnnotations;
using System.IO;
using PKHeX.Core;
using System;
using CoreAPI.Models;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoreAPI.Controllers
{
    [ApiController]
    public class PokemonInfoController : ControllerBase
    {
        // POST: api/PokemonInfo
        [HttpPost]
        [Route("api/[controller]")]
        public string Post([FromForm] [Required] IFormFile pokemon, [FromForm] string generation)
        {
            using var memoryStream = new MemoryStream();
            pokemon.CopyTo(memoryStream);
            byte[] data = memoryStream.ToArray();
            PKM pkm;
            try
            {
                if (generation == "" || generation == null)
                {
                    pkm = PKMConverter.GetPKMfromBytes(data);
                    if (pkm == null)
                    {
                        throw new System.ArgumentException("Bad data!");
                    }
                    generation = Utils.GetGeneration(pkm);
                }
                else
                {
                    //Console.WriteLine(generation
                    pkm = Utils.GetPKMwithGen(generation, data);
                    if (pkm == null)
                    {
                        throw new System.ArgumentException("Bad generation!");
                    }
                }
                Console.WriteLine(generation);
                Console.WriteLine(pkm.Species);
                Console.WriteLine(pkm.GetType());
                if (!Utils.PokemonExistsInGeneration(generation, pkm.Species))
                {
                    Response.StatusCode = 400;
                    return null;
                }
                DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };

                PokemonSummary PS = new PokemonSummary(pkm, GameInfo.Strings);
                return JsonConvert.SerializeObject(PS, new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                });
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
        // POST: api/BasePokemon
        [HttpPost]
        [Route("api/BasePokemon")]
        public BasePokemon BasePokemon([FromForm] [Required] string pokemon, [FromForm] string form)
        {
            if (!Enum.GetNames(typeof(Species)).Any(s => s.ToLower() == pokemon))
            {
                Response.StatusCode = 400;
                return null;
            }
            try
            {
                Species s = (Species)Enum.Parse(typeof(Species), pokemon, true);
                var formNum = 0;
                if (form != null)
                {
                    var forms = FormConverter.GetFormList((int)s, GameInfo.Strings.Types, GameInfo.Strings.forms, GameInfo.GenderSymbolASCII, 8);
                    formNum = StringUtil.FindIndexIgnoreCase(forms, form);
                    if (formNum < 0 || formNum >= forms.Length)
                    {
                        Response.StatusCode = 400;
                        return null;
                    }
                }




                var bp = Utils.GetBasePokemon((int)s, formNum);
                return bp;
            }
            catch
            {
                Response.StatusCode = 400;
                return null;
            }
        }
        // POST: api/GetForms
        [HttpPost]
        [Route("api/PokemonForms")]
        public string[] GetPokemonForms([FromForm] [Required] string pokemon)
        {
            if (!Enum.GetNames(typeof(Species)).Any(s => s.ToLower() == pokemon))
            {
                Response.StatusCode = 400;
                return null;
            }
            try
            {
                Species s = (Species)Enum.Parse(typeof(Species), pokemon, true);
                Utils.GetFormList((int)s);
                return Utils.GetFormList((int)s);
            }
            catch
            {
                return null;
            }
        }
    }
}