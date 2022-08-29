using System;
namespace LineBotMessage.Dtos
{

    public class MessageEventObjectsDto
    {
        public string Id { get; set; }
        public string Type { get; set; }

        // Text Message Event
        public string? Text { get; set; }
        public TextMessageEventEmojiDto? Emojis { get; set; }
        public TextMessageEventMentionDto? Mentionees { get; set; }

        // Image & Video & Audio Message Event
        public ContentProviderDto? ContentProvider { get; set; }
        public ImageMessageEventImageSetDto? ImageSet { get; set; }
        public int? Duration { get; set; }

        //File Message Event
        public string? FileName { get; set; }
        public int? FileSize { get; set; }

        //Location Message Event
        public string? Title { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Sticker Message Event
        public string? PackageId { get; set; }
        public string? StickerId { get; set; }
        public string? StickerResourceType { get; set; }
        public List<string>? Keywords { get; set; }
    }
    public class TextMessageEventEmojiDto
    {
        public int Index { get; set; }
        public int Length { get; set; }
        public string ProductId { get; set; }
        public string EmojiId { get; set; }
    }
    public class TextMessageEventMentionDto
    {
        public int Index { get; set; }
        public int Length { get; set; }
        public string UserId { get; set; }
    }
    public class ContentProviderDto
    {
        public string Type { get; set; }
        public string? OriginalContentUrl { get; set; }
        public string? PreviewImageUrl { get; set; }
    }
    public class ImageMessageEventImageSetDto
    {
        public string Id { get; set; }
        public string Index { get; set; }
        public string Total { get; set; }
    }
}

