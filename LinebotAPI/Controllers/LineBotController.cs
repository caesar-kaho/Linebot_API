using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace LinebotAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LineBotController : ControllerBase
    {

        // 貼上 messaging api channel 中的 accessToken & secret
        private readonly string channelAccessToken = "/PwKqaK8nQ02MVsw2FnSZ6q0bCjrwaMUZz4LUhqbXvXeAzC3urypi8c1/2BfTxwpXzKUaVtyy44t4gUeUqMYGbO2XY+MXG1brtgCLqYFlZt0SiWgl0j7xx1xLOEHl+MPz4ehhIjIb7/p2nZkD6w3oAdB04t89/1O/w1cDnyilFU=";
        private readonly string channelSecret = "d6cb0eeb502d5bac960b89a55f249cd4";

        // constructor
        public LineBotController()
        {

        }

        // 使用 Post 方法的原因是因為這支 API 會接收 Line 傳送的 webhook event，
        // 這部分在下一篇會介紹～
        [HttpPost("Webhook")]
        public IActionResult Webhook()
        {
            return Ok();
        }
    }
}