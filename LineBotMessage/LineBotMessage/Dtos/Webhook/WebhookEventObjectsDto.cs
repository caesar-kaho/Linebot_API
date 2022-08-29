using System;
namespace LineBotMessage.Dtos
{
    public class WebhookEventObjectsDto
    {
        public string Type { get; set; } // 事件類型
        public string Mode { get; set; } // Channel state : active | standby
        public long Timestamp { get; set; } // 事件發生時間 : event occurred time in milliseconds
        public SourceDto Source { get; set; } // 事件來源 : user | group chat | multi-person chat
        public string WebhookEventId { get; set; } // webhook event id - ULID format
        public DeliverycontextDto DeliveryContext { get; set; } // 是否為重新傳送之事件 DeliveryContext.IsRedelivery : true | false
        public string? ReplyToken { get; set; } // 回覆此事件所使用的 token
        public MessageEventObjectsDto? Message { get; set; } // 收到訊息的事件，可收到 text、sticker、image、file、video、audio、location 訊息

        public PostbackDto? Postback { get; set; }
        public VideoPlayCompleteDto? VideoPlayComplete { get; set; }
        public LinkDto? Link { get; set; }
    }

    public class SourceDto
    {
        public string Type { get; set; }
        public string UserId { get; set; }
    }

    public class DeliverycontextDto
    {
        public bool IsRedelivery { get; set; }

    }

    public class PostbackDto
    {
        public string? Data { get; set; }
        public PostbackParam? @Params { get; set; } // 變數名稱前加上 “＠” 即可使用保留字當變數名稱 
    }

    public class PostbackParam
    {
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? Datetime { get; set; }
        public string? NewRichMenuAliasId { get; set; }
        public string? Status { get; set; }
    }

    public class VideoPlayCompleteDto
    {
        public string TrackingId { get; set; }
    }

    public class LinkDto
    {
        public string Result { get; set; }
        public string Nonce { get; set; }
    }
}

