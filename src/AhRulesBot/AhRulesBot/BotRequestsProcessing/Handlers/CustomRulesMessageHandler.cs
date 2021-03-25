using AhRulesBot.MessageProcessing.Interfaces;
using AhRulesBot.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;

namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    internal class CustomRulesMessageHandler : IMessageHandler
    {
        private readonly IMessageHandler _next;
        private readonly ChannelReader<List<CustomRuleItem>> _channel;
        private readonly ILogger _logger;

        private List<CustomRuleItem> CustomRules = new List<CustomRuleItem>();

        public CustomRulesMessageHandler(
            ILogger logger,
            ChannelReader<List<CustomRuleItem>> channel,
            IMessageHandler next)
        {
            _logger = logger;
            _channel = channel;
            _next = next;
        }

        public HandlerResult Handle(string message)
        {
            var command = TryParseAsCustomRulesRequest(message);
            if (command)
            {
                return new HandlerResult { Data = ProcessRulesRequest(message) };
            }

            return _next != null ? _next.Handle(message) : new HandlerResult();
        }

        private List<string> ProcessRulesRequest(string message)
        {
            Func<CustomRuleItem, bool> messageContains = item => item.Title.Contains(message, StringComparison.InvariantCultureIgnoreCase);
            Func<CustomRuleItem, bool> messageExact = item => item.Title.Equals(message, StringComparison.InvariantCultureIgnoreCase);

            _channel.TryRead(out List<CustomRuleItem> rulesUpdate);
            if (rulesUpdate != null)
            {
                CustomRules = rulesUpdate;
            }

            return CustomRules.Where(messageContains)
                .OrderByDescending(messageExact)
                .ThenBy(i => message.CalcLevenshteinDistance(i.Title))
                .Select(i => $"<b>{i.Title}</b>\n{i.Text}").ToList();
        }

        private bool TryParseAsCustomRulesRequest(string message)
        {
            return true;
        }
    }
}
