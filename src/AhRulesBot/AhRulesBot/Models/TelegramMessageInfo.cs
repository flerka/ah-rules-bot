using System;

namespace AhRulesBot.Models
{
    internal class TelegramMessageInfo
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public TimeSpan Ttl { get; set; }
        public DateTime SentUtc { get; set; }
    }
}
