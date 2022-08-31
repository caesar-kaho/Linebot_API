namespace LineBotMessage.Dtos
{
    public class WebhookEventObjectsDto
    {
        public string Type { get; set; } // 事件類型
        public string Mode { get; set; } // Channel state : active | standby
        public long Timestamp { get; set; } // 事件發生時間 : event occurred time in milliseconds
        public SourceDto Source { get; set; } // 事件來源 : user | group chat | multi-person chat
        public string WebhookEventId { get; set; } // webhook event id - ULID format
        public DeliveryContextDto DeliveryContext { get; set; } // 是否為重新傳送之事件 DeliveryContext.IsRedelivery : true | false
        public string? ReplyToken { get; set; } // 回覆此事件所使用的 token
        public MessageEventObjectsDto? Message { get; set; } // 收到訊息的事件，可收到 text、sticker、image、file、video、audio、location 訊息
        public UnsendEventObjectDto? Unsend { get; set; } //使用者“收回”訊息事件
        
        public MemberEventObjectDto? joined { get; set; } // Memmber Joined Event
        public MemberEventObjectDto? left { get; set; } // Member Leave Event
        public PostbackEventObjectDto? Postback { get; set; } // Postback Event
        public VideoViewingCompleteEventObjectDto? VideoPlayComplete { get; set; } // Video viewing complete event
    }

    public class SourceDto
    {
        public string Type { get; set; }
        public string? UserId { get; set; }
        public string? GroupId { get; set; }
        public string? RoomId { get; set; }
    }

    public class DeliveryContextDto
    {
        public bool IsRedelivery { get; set; }

    }
}

