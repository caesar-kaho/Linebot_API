using System;
namespace LineBotMessage.Dtos
{
    public class BroadcastingMessageRequestDto<T>
    {
        public List<T> Messages { get; set; }
    }
}

