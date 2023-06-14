﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace LinebotAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LineBotController : ControllerBase
    {

        // 貼上 messaging api channel 中的 accessToken & secret
        private readonly string channelAccessToken = "";
        private readonly string channelSecret = "";

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