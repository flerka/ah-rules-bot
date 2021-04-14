using System.Collections.Generic;

namespace AhRulesBot.Models
{
    public class RuleItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
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
