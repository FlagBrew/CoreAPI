
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Helpers;
using System.ComponentModel.DataAnnotations;
using System.IO;
using PKHeX.Core;
using System;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonInfoController : ControllerBase
    {
        // POST: api/PokemonInfo
        [HttpPost]
        public PokemonSummary Post([FromForm] [Required] IFormFile pokemon)
        {
            using var memoryStream = new MemoryStream();
            pokemon.CopyTo(memoryStream);
            byte[] data = memoryStream.ToArray();
            try
            {
                var pkm = PKMConverter.GetPKMfromBytes(data);
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
    }
}