using LineBotMessage.Dtos;

namespace LineBotMessage.Domain.Interfaces
{
    public interface ILineBotService
    {
        public void ReceiveWebhook(WebhookRequestBodyDto requestBody);
    }
}

