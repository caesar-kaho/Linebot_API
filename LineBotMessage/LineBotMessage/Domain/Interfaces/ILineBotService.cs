using LineBotMessage.Dtos;

namespace LineBotMessage.Services.Interfaces
{
    public interface ILineBotService
    {
        public void ReceiveWebhook(WebhookRequestBodyDto requestBody);
    }
}

