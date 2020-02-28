using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreAPI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PKHeX.Core;

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
            return "hello world!";
        }
    }
}