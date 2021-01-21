using System.Collections.Generic;

namespace AhRulesBot
{
    public class RuleItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }

    public class SubRule : RuleItem
    {
        public IList<RuleItem> Rules { get; set; }
    }

    public class Rule : RuleItem
    {
        public IList<SubRule> Rules { get; set; }
    }
}
