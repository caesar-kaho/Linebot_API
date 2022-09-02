using LineBotMessage.Dtos;
using LineBotMessage.Domain;
using Microsoft.AspNetCore.Mvc;

namespace LineBotMessage.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class LineBotController : ControllerBase
    {

        private readonly LineBotService _lineBotService; 
        // constructor
        public LineBotController()
        {
            _lineBotService = new LineBotService();
        }

        [HttpPost("Webhook")]
        public IActionResult Webhook(WebhookRequestBodyDto body)
        {
            _lineBotService.ReceiveWebhook(body);
            return Ok();
        }

        [HttpPost("Message/Broadcast")]
        public IActionResult Broadcast(object body)
        {
            _lineBotService.BroadcastMessageReqeust(body as BroadcastingMessageRequestDto);
            return Ok();
        }
    }
}