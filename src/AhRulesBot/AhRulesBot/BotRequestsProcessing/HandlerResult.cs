using System;
using System.Collections.Generic;

namespace AhRulesBot.BotRequestsProcessing
{
    internal class HandlerResult
    {
        public List<HandlerResultData> Data { get; set; } = new List<HandlerResultData>();
        public TimeSpan? Ttl { get; set; }
    }

    internal class HandlerResultData
    {
        public string? Text { get; set;}
        public string? TelegramImageUrl { get; set; }
        public string? TelegramStickerId { get; set; }
    } 
}
