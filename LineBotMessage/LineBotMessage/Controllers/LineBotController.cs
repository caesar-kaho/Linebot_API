using Microsoft.AspNetCore.Mvc;

namespace LineBotMessage.Controllers
{

    [Route("api/v{version:apiVersion}/[Controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class LineBotController : ControllerBase
    {

        // 貼上 messaging api channel 中的 accessToken & secret
        private readonly string channelAccessToken = "Your channel access token";
        private readonly string channelSecret = "Your channel secret";

        // constructor
        public LineBotController()
        {

        }

        [HttpPost("Webhook")]
        public IActionResult Webhook()
        {
            return Ok();
        }

    }
}