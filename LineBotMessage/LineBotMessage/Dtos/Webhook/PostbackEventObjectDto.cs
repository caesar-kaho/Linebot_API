namespace LineBotMessage.Dtos
{
    public class PostbackEventObjectDto
    {
        public string? Data { get; set; }
        public PostbackEventParamDto? Params { get; set; } 
    }

    public class PostbackEventParamDto
    {
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? Datetime { get; set; }
        public string? NewRichMenuAliasId { get; set; }
        public string? Status { get; set; }
    }
}

