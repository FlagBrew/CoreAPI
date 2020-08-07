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