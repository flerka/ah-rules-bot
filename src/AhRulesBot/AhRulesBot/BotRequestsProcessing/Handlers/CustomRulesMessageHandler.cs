using AhRulesBot.MessageProcessing.Interfaces;
using AhRulesBot.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;

namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    public class CustomRulesMessageHandler : IMessageHandler
    {
        private IMessageHandler _next;
        private readonly ChannelReader<List<CustomRuleItem>> _channel;
        private List<CustomRuleItem> CustomRules = new List<CustomRuleItem>();
        private readonly ILogger _logger;

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
                var result = new HandlerResult();
                result.Data = ProcessRulesRequest(message);
                return result;
            }

            return _next != null ? _next.Handle(message) : new HandlerResult();
        }

        private List<string> ProcessRulesRequest(string message)
        {
            Func<CustomRuleItem, bool> messageContains = item => item.Title.Contains(message, StringComparison.InvariantCultureIgnoreCase);
            Func<CustomRuleItem, bool> messageExact = item => item.Title.Equals(message, StringComparison.InvariantCultureIgnoreCase);

            _channel.TryRead(out CustomRules);

            return CustomRules.Where(messageContains)
                .OrderByDescending(messageExact)
                //.ThenBy(i => CalcLevenshteinDistance(message, i.Title))
                .Select(i => $"<b>{i.Title}</b>\n{i.Text}").ToList();
        }

        private bool TryParseAsCustomRulesRequest(string message)
        {
            return true;
        }
    }
}
