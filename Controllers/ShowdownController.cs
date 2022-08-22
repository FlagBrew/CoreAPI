using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PKHeX.Core;
using CoreAPI.Helpers;
using PKHeX.Core.AutoMod;
using CoreAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CoreAPI.Controllers
{
    [ApiController]
    public class ShowndownController : ControllerBase
    {
        // POST api/Showdown
        [Route("api/Showdown")]
        [HttpPost]
        public dynamic Showdown([FromForm] [Required] string set, [FromForm] string generation)
        {
            ShowdownSet showdown;
            try
            {
                showdown = new ShowdownSet(set);
                Console.WriteLine(Utils.GetGameVersion(generation));
                var sav = SaveUtil.GetBlankSAV(Utils.GetGameVersion(generation), "Yeet");
                PKM newPKM = sav.GetLegalFromSet(showdown, out var result);
                PokemonSummary pks = new PokemonSummary(newPKM, GameInfo.Strings);
                if (pks.Species == "")
                {
                    throw new System.ArgumentException("Your Pokemon does not exist!");
                }

                DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };


                Response.ContentType = "application/json";
                return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(pks, new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                }));

            } catch (Exception e) {
                dynamic error = new JObject();
                error.message = e.Message;
                return StatusCode(400, error);
            }
        }
    }
}