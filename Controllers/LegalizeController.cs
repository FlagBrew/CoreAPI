using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Models;
using CoreAPI.Helpers;
using System.IO;
using System.ComponentModel.DataAnnotations;
using PKHeX.Core;
using System.Reflection.Metadata.Ecma335;

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
                if (generation == "" || generation == null)
                {
                    pkm = PKMConverter.GetPKMfromBytes(data);
                    if (pkm == null)
                    {
                        throw new System.ArgumentException("Bad data!");
                    }
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

            if (version == "" || version == null)
            {
                version = Utils.GetGameVersion(pkm).ToString();
            }

                Legalize L = new Legalize(pkm, version);
            return L;
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
                if (generation == "" || generation == null)
                {
                    pkm = PKMConverter.GetPKMfromBytes(data);
                    if (pkm == null)
                    {
                        throw new System.ArgumentException("Bad data!");
                    }
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