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

        private static HttpClient client = new HttpClient();
        private readonly JsonProvider _jsonProvider = new JsonProvider();

        public RichMenuService()
        {
        }

        public void ValidateRichMenu(RichMenuDto richMenu)
        {
           
        }

        public void CreateRichMenu(RichMenuDto richMenu)
        {

        }

        public void UploadRichMenuImage(string richMenuId, IFormFile image)
        {

        }

        public void SetDefaultRichMenu(string richMenuId)
        {

        }
    }
}

