
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Helpers;
using System.ComponentModel.DataAnnotations;
using System.IO;
using PKHeX.Core;
using System;
using CoreAPI.Models;
using System.Linq;

namespace CoreAPI.Controllers
{
    [ApiController]
    public class PokemonInfoController : ControllerBase
    {
        // POST: api/PokemonInfo
        [HttpPost]
        [Route("api/[controller]")]
        public PokemonSummary Post([FromForm] [Required] IFormFile pokemon, [FromForm] string generation)
        {
            using var memoryStream = new MemoryStream();
            pokemon.CopyTo(memoryStream);
            byte[] data = memoryStream.ToArray();
            PKM pkm;
            var gen = 0;
            try
            {
                if (generation != null)
                {
                    try
                    {
                        gen = Int32.Parse(generation);
                    } catch
                    {
                        throw new System.ArgumentException("Bad data!");
                    }
                }

                if (gen != 0)
                {
                    pkm = PKMConverter.GetPKMfromBytes(data, gen);
                }
                else
                {
                    pkm = PKMConverter.GetPKMfromBytes(data);
                }
                if (pkm == null)
                {
                    throw new System.ArgumentException("Bad data!");
                }
                PokemonSummary PS = new PokemonSummary(pkm, GameInfo.Strings);
                return PS;
            }
            catch
            {
                return null;
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