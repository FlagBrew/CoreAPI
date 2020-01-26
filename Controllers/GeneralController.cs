using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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