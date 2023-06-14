using LineBotMessage.Enum;

namespace LineBotMessage.Dtos
{
    public class StickerMessageDto : BaseMessageDto
    {
        public StickerMessageDto()
        {
            Type = MessageTypeEnum.Sticker;
        }
        public string PackageId { get; set; }
        public string StickerId { get; set; }
    }
}

