using LineBotMessage.Domain;
using LineBotMessage.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LineBotMessage.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class LineLoginController : ControllerBase
    {
        private readonly LineLoginService _lineLoginService;
        public LineLoginController()
        {
            _lineLoginService = new LineLoginService();
        }

        // 取得 Line Login 網址
        [HttpGet("Url")]
        public string GetLoginUrl([FromQuery] string redirectUrl)
        {
            return _lineLoginService.GetLoginUrl(redirectUrl);
        }

        // 使用 authToken 取回登入資訊
        [HttpGet("Tokens")]
        public async Task<TokensResponseDto> GetTokensByAuthToken([FromQuery] string authToken, [FromQuery] string callbackUrl)
        {
            return await _lineLoginService.GetTokensByAuthToken(authToken, callbackUrl);
        }

        // 使用 auth token 取得 user profile
        [HttpGet("Profile/ByAuthToken")]
        public async Task<UserProfileDto> GetUserProfileByAuthToken([FromQuery] string authToken, [FromQuery] string callbackUrl)
        {
            return await _lineLoginService.GetUserProfileByAuthToken(authToken, callbackUrl);
        }

        // 使用 access token 取得 user profile
        [HttpGet("Profile/{accessToken}")]
        public async Task<UserProfileDto> GetUserProfileByAccessToken(string accessToken)
        {
            return await _lineLoginService.GetUserProfileByAccessToken(accessToken);
        }

        // 使用 id token 取得 user profile
        [HttpGet("Profile/IdToken/{idToken}")]
        public async Task<UserIdTokenProfileDto> GetUserProfileByIdToken(string idToken)
        {
            return await _lineLoginService.GetUserProfileByIdToken(idToken);
        }
    }
}

