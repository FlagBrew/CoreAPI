using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Models;
using System.ComponentModel.DataAnnotations;
using CoreAPI.Helpers;
using System.Collections.Generic;

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
                var acceptableGenerations = new List<String>() { "1", "2", "3", "4", "5", "6", "7", "8", "BDSP", "PLA"};
                if (acceptableGenerations.FirstOrDefault(g => g == generation.ToUpper()) == string.Empty)
                {
                    throw new ArgumentOutOfRangeException("Must be between 1 and PLA");
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
