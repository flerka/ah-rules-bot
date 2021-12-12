using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AhRulesBot.Models
{
    public class RuleItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        [JsonPropertyName("telegram_img_url")]
        public string? TelegramImgUrl { get; set; }
    }

    public class SubRule : RuleItem
    {
        public List<RuleItem> Rules { get; set; } = new();
    }

    public class Rule : RuleItem
    {
        public List<SubRule> Rules { get; set; } = new();
    }
}
