using LineBotMessage.Dtos;
using LineBotMessage.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LineBotMessage.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    public class LineBotController : ControllerBase
    {

        private readonly ILineBotService _lineBotService; 
        // constructor
        public LineBotController(ILineBotService lineBotService)
        {
            _lineBotService = lineBotService; // 注入 LineBotService
        }

        [HttpPost("Webhook")]
        public IActionResult Webhook(WebhookRequestBodyDto body)
        {
            _lineBotService.ReceiveWebhook(body);
            return Ok();
        }

    }
}