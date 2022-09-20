using System.Net.Http.Headers;
using LineBotMessage.Dtos;
using LineBotMessage.Enum;
using LineBotMessage.Providers;
using System.Text;

namespace LineBotMessage.Domain
{
    public class RichMenuService
    {
        // 貼上 messaging api channel 中的 accessToken & secret
        private readonly string channelAccessToken = "gCEru16JH8CSHv+YoIXiCDD+vac9RAiIr/eJaXL4ZbRaRhwJdpJa8Uhd59DoXAjAXEvXYXbTCnIScSxl7ek2S/rV4LHBaxXt4I4bgSsuWM0gu9vncuOxFZ9odba9x7J0+P7j9ioVFweZe/Dhfq8fcwdB04t89/1O/w1cDnyilFU=";
        private readonly string channelSecret = "7b79ab80c255e148755672de6e73583b";

        private readonly string validateRichMenuUri = "https://api.line.me/v2/bot/richmenu/validate";
        private readonly string createRichMenuUri = "https://api.line.me/v2/bot/richmenu";
        private readonly string getRichMenuListUri = "https://api.line.me/v2/bot/richmenu/list";
        private readonly string uploadRichMenuImageUri = "https://api-data.line.me/v2/bot/richmenu/{0}/content";
        private readonly string setDefaultRichMenuUri = "https://api.line.me/v2/bot/user/all/richmenu/{0}";

        private readonly string createRichMenuAliasUri = "https://api.line.me/v2/bot/richmenu/alias";
        private readonly string commonRichMenuAliasUri = "https://api.line.me/v2/bot/richmenu/alias/{0}";
        private readonly string getRichMenuAliasListUri = "https://api.line.me/v2/bot/richmenu/alias/list";

        private static HttpClient client = new HttpClient();
        private readonly JsonProvider _jsonProvider = new JsonProvider();

        public RichMenuService()
        {
        }

        public async Task<string> ValidateRichMenu(RichMenuDto richMenu)
        {
            var jsonBody = new StringContent(_jsonProvider.Serialize(richMenu), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(validateRichMenuUri),
                Content = jsonBody,
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CreateRichMenu(RichMenuDto richMenu)
        {
            var jsonBody = new StringContent(_jsonProvider.Serialize(richMenu), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(createRichMenuUri),
                Content = jsonBody,
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<RichMenuListDto> GetRichMenuList()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(getRichMenuListUri),
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
            var response = await client.SendAsync(request);

            Console.WriteLine(await response.Content.ReadAsStringAsync());
            var list = _jsonProvider.Deserialize<RichMenuListDto>(await response.Content.ReadAsStringAsync());
            // 依照名稱排序
            list.Richmenus = list.Richmenus.OrderBy((rm) => rm.Name).ToList();
            return list;
        }

        public async Task<string> UploadRichMenuImage(string richMenuId, IFormFile imageFile)
        {
            //判斷檔案格式 需為 png or jpeg
            if (!(Path.GetExtension(imageFile.FileName).Equals(".png", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(imageFile.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase)))
            {
                return "圖片格式錯誤，須為 png or jpeg";
            }
            using (var stream = new MemoryStream())
            {
                //建立檔案內容
                imageFile.CopyTo(stream);
                var fileBytes = stream.ToArray();
                var content = new ByteArrayContent(fileBytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                var request = new HttpRequestMessage(HttpMethod.Post, string.Format(uploadRichMenuImageUri, richMenuId))
                {
                    Content = content
                };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
                var response = await client.SendAsync(request);

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> SetDefaultRichMenu(string richMenuId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, string.Format(setDefaultRichMenuUri, richMenuId));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        // Rich menu alias
        public async Task<string> CreateRichMenuAlias(RichMenuAliasDto richMenuAlias)
        {

            var jsonBody = new StringContent(_jsonProvider.Serialize(richMenuAlias), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
            var request = new HttpRequestMessage(HttpMethod.Post, createRichMenuAliasUri)
            {
                Content = jsonBody
            };

            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> DeleteRichMenuAlias(string richMenuAliasId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, string.Format(commonRichMenuAliasUri, richMenuAliasId));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UpdateRichMenuAlias(string richMenuAliasId, string richMenuId)
        {
            var body = new { richMenuId = richMenuId };
            var jsonBody = new StringContent(_jsonProvider.Serialize(body), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
            var request = new HttpRequestMessage(HttpMethod.Post, string.Format(commonRichMenuAliasUri, richMenuAliasId))
            {
                Content = jsonBody
            };

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<RichMenuAliasDto> GetRichMenuAliasInfo(string richMenuAliasId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format(commonRichMenuAliasUri, richMenuAliasId));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return _jsonProvider.Deserialize<RichMenuAliasDto>(await response.Content.ReadAsStringAsync());
            else
                return new RichMenuAliasDto();
        }

        public async Task<RichMenuAliasListDto> GetRichMenuAliasListInfo()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, getRichMenuAliasListUri);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return _jsonProvider.Deserialize<RichMenuAliasListDto>(await response.Content.ReadAsStringAsync());
            else
                return new RichMenuAliasListDto();
        }
    }
}

