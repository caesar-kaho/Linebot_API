using LineBotMessage.Enum;
namespace LineBotMessage.Dtos
{
    public class TemplateMessageDto<T> : BaseMessageDto
    {
        public TemplateMessageDto()
        {
            Type = MessageTypeEnum.Template;
        }

        public string AltText { get; set; }
        public T Template { get; set; }
    }

    public class ButtonsTemplateDto
    {
        public string Type { get; set; } = TemplateTypeEnum.Buttons;
        public string Text { get; set; }
        public List<ActionDto>? Actions { get; set; }

        public string? ThumbnailImageUrl { get; set; }
        public string? ImageAspectRatio { get; set; }
        public string? ImageSize { get; set; }
        public string? ImageBackgroundColor { get; set; }
        public string? Title { get; set; }
        public string? DefaultAction { get; set; }
    }

    public class ConfirmTemplateDto
    {
        public string Type { get; set; } = TemplateTypeEnum.Confirm;
        public string Text { get; set; }
        public List<ActionDto>? Actions { get; set; }
    }
}

