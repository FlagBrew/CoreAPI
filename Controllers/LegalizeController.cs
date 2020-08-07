using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Models;
using CoreAPI.Helpers;
using System.IO;
using System.ComponentModel.DataAnnotations;
using PKHeX.Core;

namespace CoreAPI.Controllers
{
    [ApiController]
    public class LegalizeController : ControllerBase
    {
        // POST: api/Legalize
        [Route("api/[controller]")]
        [HttpPost]
        public Legalize Legalize([FromForm] [Required] IFormFile pokemon, [FromForm] string version, [FromForm] string generation)
        {
            using var memoryStream = new MemoryStream();
            pokemon.CopyTo(memoryStream);
            PKM pkm;
            byte[] data = memoryStream.ToArray();
            try
            {
                if (string.IsNullOrEmpty(generation))
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
                    pkm = Utils.GetPKMwithGen(generation, data);
                    if (pkm == null)
                    {
                        throw new System.ArgumentException("Bad generation!");
                    }
                }
            }
            catch
            {
                Response.StatusCode = 400;
                return null;
            }

            if (string.IsNullOrEmpty(version))
            {
                version = Utils.GetGameVersion(pkm).ToString();
            }
            if (!Utils.PokemonExistsInGeneration(generation, pkm.Species))
            {
                Response.StatusCode = 400;
                return null;
            }
            return new Legalize(pkm, version);
        }
        // POST: api/LegalityCheck 
        [Route("api/LegalityCheck")]
        [HttpPost]
        public string CheckLegality([FromForm] [Required] IFormFile pokemon, [FromForm] string generation)
        {
            using var memoryStream = new MemoryStream();
            pokemon.CopyTo(memoryStream);
            PKM pkm;
            byte[] data = memoryStream.ToArray();
            try
            {
                if (string.IsNullOrEmpty(generation))
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
                    pkm = Utils.GetPKMwithGen(generation, data);
                    if (pkm == null)
                    {
                        throw new System.ArgumentException("Bad generation!");
                    }
                }
            }
            catch
            {
                Response.StatusCode = 400;
                return null;
            }

            if (!Utils.PokemonExistsInGeneration(generation, pkm.Species))
            {
                Response.StatusCode = 400;
                return null;
            }

            var la = new LegalityAnalysis(pkm);
            return la.Report();
        }
        // ignore the following weird spagehti codes, but piepie62 nearly drove me to drinkin on trying to come up with a solution.
        [Route("pksm/legality/check")]
        [HttpPost]
        public string clRoute([FromForm] [Required] IFormFile pkmn, [FromHeader] string Generation)
        {
            return CheckLegality(pkmn, Generation);
        }

        [Route("api/v1/bot/auto_legality")]
        [HttpPost]
        public Legalize alRoute([FromForm] [Required] IFormFile pkmn, [FromHeader] string version, [FromHeader] string Generation)
        {
            return Legalize(pkmn, version, Generation);
        }
    }
}