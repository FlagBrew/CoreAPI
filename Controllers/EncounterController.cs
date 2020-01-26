using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Models;
using System.ComponentModel.DataAnnotations;
using CoreAPI.Helpers;
namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncounterController : ControllerBase
    {
        // POST: api/Encounter
        [HttpPost]
        public Encounter Post([FromForm] [Required] string query, [FromForm] [Required] string generation)
        {
            var data = Utils.SplitQueryString(Utils.FixQueryString(query));
            if (data.Length < 1)
            {
                Response.StatusCode = 400;
                return null;
            }
            // Make sure the generation is between 1 and 8 and is actually a number
            try
            {
                var gen = Int32.Parse(generation);
                if(gen < 1 || gen > 8)
                {
                    throw new ArgumentOutOfRangeException("Must be between 1 and 8");
                }
            } catch
            {
                Response.StatusCode = 400;
                return null;
            }
            Encounter enc = new Encounter(data[0], generation, data.Skip(1).ToArray());
            return enc;
        }
    }
}