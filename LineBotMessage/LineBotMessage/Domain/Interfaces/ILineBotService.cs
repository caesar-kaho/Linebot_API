using LineBotMessage.Dtos;

namespace LineBotMessage.Domain.Interfaces
{
    public interface ILineBotService
    {
        public void ReceiveWebhook(WebhookRequestBodyDto requestBody);
        public void ReplyMessage<T>(ReplyMessageRequestDto<T> request);
        public void BroadcastMessageReqeust(BroadcastingMessageRequestDto requestBody);
        public void BroadcastMessage(BroadcastingMessageRequestDto request);
    }
}

