using LineBotMessage.Dtos;
using LineBotMessage.Domain;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using LineBotMessage.Enum;
using LineBotMessage.Providers;

namespace LineBotMessage.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class LineBotController : ControllerBase
    {

        private readonly LineBotService _lineBotService;
        private readonly RichMenuService _richMenuService;
        private readonly JsonProvider _jsonProvider;
        // constructor
        public LineBotController()
        {
            _lineBotService = new LineBotService();
            _richMenuService = new RichMenuService();
            _jsonProvider = new JsonProvider();
        }

        [HttpPost("Webhook")]
        public IActionResult Webhook(WebhookRequestBodyDto body)
        {
            _lineBotService.ReceiveWebhook(body);
            return Ok();
        }

        [HttpPost("SendMessage/Broadcast")]
        public IActionResult Broadcast([Required] string messageType, object body)
        {
            _lineBotService.BroadcastMessageHandler(messageType, body);
            return Ok();
        }

        //rich menu api
        [HttpPost("RichMenu/Validate")]
        public IActionResult ValidateRichMenu(RichMenuDto richMenu)
        {
            _richMenuService.ValidateRichMenu(richMenu);
            return Ok();
        }

        [HttpPost("RichMenu/Create")]
        public IActionResult CreateRichMenu(RichMenuDto richMenu)
        {
            _richMenuService.CreateRichMenu(richMenu);
            return Ok();
        }

        [HttpPost("RichMenu/UploadImage/{richMenuId}")]
        public IActionResult UploadRichMenuImage([FromForm] IFormFile imageFile, [FromForm] string richMenuId)
        {
            _richMenuService.UploadRichMenuImage(richMenuId, imageFile);
            return Ok();
        }

        [HttpPost("RichMenu/SetDefault/{richMenuId}")]
        public IActionResult SetDefaultRichMenu(string richMenuId)
        {
            _richMenuService.SetDefaultRichMenu(richMenuId);
            return Ok();
        }
    }
}