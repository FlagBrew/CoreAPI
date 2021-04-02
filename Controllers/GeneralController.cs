using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;

namespace CoreAPI.Controllers
{
    [ApiController]
    public class GeneralController : ControllerBase
    {
        // GET api/Ping
        [Route("api/Ping")]
        [HttpGet]
        public string Pong()
        {
            Version.TryParse(PKHeX.Core.AutoMod.ALMVersion.CurrentVersion, out var pkhex_version);

            Response.Headers.Add("X-PKHeX-Version", pkhex_version.ToString());
            Response.Headers.Add("X-ALM-Version", pkhex_version.ToString());
            return "hello world!";
        }
    }
}