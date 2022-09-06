using LineBotMessage.Enum;
namespace LineBotMessage.Dtos
{
    public class AudioMessageDto : BaseMessageDto
    {
        public AudioMessageDto()
        {
            Type = MessageTypeEnum.Audio;
        }

        public string OriginalContentUrl { get; set; }
        public int Duration { get; set; }
    }
}

