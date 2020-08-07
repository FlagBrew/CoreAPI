using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PKHeX.Core;
using CoreAPI.Models;
using CoreAPI.Helpers;
using System.ComponentModel.DataAnnotations;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearnableMoveController : ControllerBase
    {
        // POST: api/LearnableMove
        [HttpPost]
        public List<LearnableMove> Post([FromForm] [Required] string query)
        {
            var data = Utils.SplitQueryString(query);
            if (data.Length < 2)
            {
                Response.StatusCode = 400;
                return null;
            }

            // Check if the Pokemon actually exists
            if (!Utils.PokemonExists(data[0]))
            {
                Response.StatusCode = 400;
                return null;
            }

            var moves = new List<LearnableMove>(4);
            foreach (var move in data.Skip(1))
            {
                var workaround = move.Split(',');
                LearnableMove lm = new LearnableMove
                {
                    Name = move,
                    Learnable = bool.Parse(EncounterLearn.CanLearn(data[0], workaround).ToString())
                };
                moves.Add(lm);
                if (moves.Count == 4)
                {
                    break;
                }
            }

            return moves;
        }
    }
}
