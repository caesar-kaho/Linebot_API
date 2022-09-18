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
        private readonly string uploadRichMenuImageUri = "https://api-data.line.me/v2/bot/richmenu/{0}/content";
        private readonly string setDefaultRichMenuUri = "https://api.line.me/v2/bot/user/all/richmenu/{0}";

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

        public async void CreateRichMenu(RichMenuDto richMenu)
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

            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        public async void UploadRichMenuImage(string richMenuId, IFormFile imageFile)
        {
            //判斷檔案格式 需為 png or jpeg
            if (!(Path.GetExtension(imageFile.FileName).Equals(".png", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(imageFile.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }
            using (var stream = new MemoryStream())
            {
                //建立檔案內容
                imageFile.CopyTo(stream);
                var fileBytes = stream.ToArray();
                var content = new ByteArrayContent(fileBytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                var request = new HttpRequestMessage(HttpMethod.Post, String.Format(uploadRichMenuImageUri, richMenuId))
                {
                    Content = content
                };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
                var response = await client.SendAsync(request);

                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        public async void SetDefaultRichMenu(string richMenuId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, String.Format(setDefaultRichMenuUri,richMenuId));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

            var response = await client.SendAsync(request);

            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}

