using LineBotMessage.Dtos;
using LineBotMessage.Domain;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using LineBotMessage.Enum;
using LineBotMessage.Providers;
using System.Net.Mime;

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
        public async Task<IActionResult> ValidateRichMenu(RichMenuDto richMenu)
        {
            return Ok(await _richMenuService.ValidateRichMenu(richMenu));
        }

        [HttpPost("RichMenu/Create")]
        public async Task<IActionResult> CreateRichMenu(RichMenuDto richMenu)
        {
            return Ok(await _richMenuService.CreateRichMenu(richMenu));
        }

        [HttpGet("RichMenu/GetList")]
        public async Task<IActionResult> GetRichMenuList()
        {
            return Ok(await _richMenuService.GetRichMenuList());
        }

        [HttpPost("RichMenu/UploadImage/{richMenuId}")]
        public async Task<IActionResult> UploadRichMenuImage(IFormFile imageFile, string richMenuId)
        {
            return Ok(await _richMenuService.UploadRichMenuImage(richMenuId, imageFile));
        }

        [HttpGet("RichMenu/SetDefault/{richMenuId}")]
        public async Task<IActionResult> SetDefaultRichMenu(string richMenuId)
        {
            return Ok(await _richMenuService.SetDefaultRichMenu(richMenuId));
        }

        [HttpGet("RichMenu/DownloadImage/{richMenuId}")]
        public async Task<FileContentResult> DownloadRichMenuImageById(string richMenuId)
        {
            return await _richMenuService.DownloadRichMenuImage(richMenuId);
        }

        [HttpDelete("RichMenu/Delete/{richMenuId}")]
        public async Task<IActionResult> DeleteRichMenu(string richMenuId)
        {
            return Ok(await _richMenuService.DeleteRichMenu(richMenuId));
        }

        [HttpGet("RichMenu/Get/{richMenuId}")]
        public async Task<IActionResult> GetRichMenuById(string richMenuId)
        {
            return Ok(await _richMenuService.GetRichMenuById(richMenuId));
        }

        [HttpGet("RichMenu/Default/GetId")]
        public async Task<IActionResult> GetDefaultRichMenuId()
        {
            return Ok(await _richMenuService.GetDefaultRichMenuId());
        }

        [HttpGet("RichMenu/Default/Cancel")]
        public async Task<IActionResult> CancelDefaultRichMenu()
        {
            return Ok(await _richMenuService.CancelDefaultRichMenu());
        }

        [HttpGet("RichMenu/Link/{userId}/{richMenuId}")]
        public async Task<IActionResult> LinkRichMenuToUser(string userId, string richMenuId)
        {
            return Ok(await _richMenuService.LinkRichMenuToUser(userId, richMenuId));
        }

        [HttpPost("RichMenu/Link/Multiple")]
        public async Task<IActionResult> LinkRichMenuToMultipleUser(LinkRichMenuToMultipleUserDto dto)
        {
            return Ok(await _richMenuService.LinkRichMenuToMultipleUser(dto));
        }

        [HttpDelete("RichMenu/Unlink/{userId}")]
        public async Task<IActionResult> UnlinkRichMenuFromUser(string userId)
        {
            return Ok(await _richMenuService.UnlinkRichMenuFromUser(userId));
        }

        [HttpPost("RichMenu/Unlink/Multiple")]
        public async Task<IActionResult> UnlinkRichMenuFromMMultipleUser(LinkRichMenuToMultipleUserDto dto)
        {
            return Ok(await _richMenuService.UnlinkRichMenuFromMultipleUser(dto));
        }

        //Rich menu alias
        [HttpPost("RichMenu/Alias/Create")]
        public async Task<IActionResult> CreateRichMenuAlias(RichMenuAliasDto richMenuAlias)
        {
            return Ok(await _richMenuService.CreateRichMenuAlias(richMenuAlias));
        }

        [HttpDelete("RichMenu/Alias/Delete/{richMenuAliasId}")]
        public async Task<IActionResult> DeleteRichMenuAlias(string richMenuAliasId)
        {
            return Ok(await _richMenuService.DeleteRichMenuAlias(richMenuAliasId));
        }

        [HttpPost("RichMenu/Alias/Upadte/{richMenuAliasId}")]
        public async Task<IActionResult> UpdateRichMenuAlias(string richMenuAliasId, string richMenuId)
        {
            return Ok(await _richMenuService.UpdateRichMenuAlias(richMenuAliasId, richMenuId));
        }

        [HttpGet("RichMenu/Alias/GetInfo/{richMenuAliasId}")]
        public async Task<IActionResult> GetRichMenuAliasInfomation(string richMenuAliasId)
        {
            return Ok(await _richMenuService.GetRichMenuAliasInfo(richMenuAliasId));
        }

        [HttpGet("RichMenu/Alias/GetInfo/List")]
        public async Task<IActionResult> GetRichMenuAliasList()
        {
            return Ok(await _richMenuService.GetRichMenuAliasListInfo());
        }
    }
}