using Microsoft.AspNetCore.Mvc;

namespace LineBotMessage.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class HelloWorldController : ControllerBase
    {
        public HelloWorldController()
        {

        }

        [HttpGet]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [HttpGet("Get")]
        public string HelloWorld2()
        {
            return "Hello World 2";
        }
    }
}