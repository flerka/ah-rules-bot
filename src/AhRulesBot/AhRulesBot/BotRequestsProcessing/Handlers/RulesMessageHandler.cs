using AhRulesBot.MessageProcessing.Interfaces;
using AhRulesBot.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    internal class RulesMessageHandler : IMessageHandler
    {
        private readonly IMessageHandler _next;
        private readonly List<RuleItem> _rules;

        public RulesMessageHandler(
            List<RuleItem> rules,
            IMessageHandler next)
        {
            _rules = rules;
            _next = next;
        }

        public HandlerResult Handle(string message)
        {
            var command = TryParseAsRulesRequest();
            if (command)
            {
                var processRequest = ProcessRulesRequest(message);
                if (processRequest != null && processRequest.Count != 0)
                {
                    return new HandlerResult { Data = processRequest };
                }
            }

            return _next != null ? _next.Handle(message) : new HandlerResult();
        }

        private List<HandlerResultData> ProcessRulesRequest(string message)
        {
            bool messageContains(RuleItem item) => item.Title.Contains(message, StringComparison.InvariantCultureIgnoreCase)
                    || item.Id.Contains(message, StringComparison.InvariantCultureIgnoreCase);
            bool messageExact(RuleItem item) => item.Title.Equals(message, StringComparison.InvariantCultureIgnoreCase)
                || item.Id.Equals(message, StringComparison.InvariantCultureIgnoreCase);

            return _rules.Where(messageContains)
                .OrderByDescending(messageExact).ThenBy(i => message.CalcLevenshteinDistance(i.Title))
                .Select(i => new HandlerResultData { Text = $"<b>{i.Title}</b>\n{i.Text}", TelegramImageUrl = i.TelegramImgUrl }).ToList();
        }

        private static bool TryParseAsRulesRequest()
        {
            return true;
        }
    }
}
