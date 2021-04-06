using AhRulesBot.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    internal class RulesMessageHandler : IMessageHandler
    {
        private IMessageHandler _next;
        private readonly List<RuleItem> _rules;
        private readonly ILogger _logger;

        public RulesMessageHandler(
            ILogger logger,
            List<RuleItem> rules,
            IMessageHandler next)
        {
            _logger = logger;
            _rules = rules;
            _next = next;
        }

        public HandlerResult Handle(string message)
        {
            var command = TryParseAsRulesRequest(message);
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

        private List<string> ProcessRulesRequest(string message)
        {
            Func<RuleItem, bool> messageContains = item => item.Title.Contains(message, StringComparison.InvariantCultureIgnoreCase)
                    || item.Id.Contains(message, StringComparison.InvariantCultureIgnoreCase);
            Func<RuleItem, bool> messageExact = item => item.Title.Equals(message, StringComparison.InvariantCultureIgnoreCase)
                || item.Id.Equals(message, StringComparison.InvariantCultureIgnoreCase);

            return _rules.Where(messageContains)
                .OrderByDescending(messageExact).ThenBy(i => message.CalcLevenshteinDistance(i.Title))
                .Select(i => $"<b>{i.Title}</b>\n{i.Text}").ToList();
        }

        private bool TryParseAsRulesRequest(string message)
        {
            return true;
        }
    }
}
