using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Models;
using System.ComponentModel.DataAnnotations;
using CoreAPI.Helpers;

namespace CoreAPI.Controllers
{
    [ApiController]
    public class EncounterController : ControllerBase
    {
        // POST: api/Encounter
        [HttpPost]
        [Route("api/[controller]")]
        public Encounter PokeInfo([FromForm] [Required] string query, [FromForm] [Required] string generation)
        {
            var data = Utils.SplitQueryString(query);
            if (data.Length == 0)
            {
                Response.StatusCode = 400;
                return null;
            }
            // Make sure the generation is between 1 and 8 and is actually a number
            try
            {
                var gen = int.Parse(generation);
                if (gen < 1 || gen > 8)
                {
                    throw new ArgumentOutOfRangeException("Must be between 1 and 8");
                }
            }
            catch
            {
                Response.StatusCode = 400;
                return null;
            }
            return new Encounter(data[0], generation, data.Skip(1).ToArray());
        }
    }
}
