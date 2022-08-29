namespace LineBotMessage.Dtos
{
    public class WebhookRequestBodyDto
    {
        public string? Destination { get; set; }
        public List<WebhookEventObjectsDto> Events { get; set; }
    }
}

