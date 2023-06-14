using LineBotMessage.Enum;

namespace LineBotMessage.Dtos
{
    public class ImageMessageDto : BaseMessageDto
    {
        public ImageMessageDto()
        {
            Type = MessageTypeEnum.Image;
        }

        public string OriginalContentUrl { get; set; }
        public string PreviewImageUrl { get; set; }
    }
}

