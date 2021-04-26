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
        private readonly IMessageHandler? _next;
        private readonly ChannelReader<List<CustomRuleItem>> _channel;

        private List<CustomRuleItem> CustomRules = new();

        public CustomRulesMessageHandler(
            ChannelReader<List<CustomRuleItem>> channel,
            IMessageHandler? next)
        {
            _channel = channel;
            _next = next;
        }

        public HandlerResult Handle(string message)
        {
            var command = TryParseAsCustomRulesRequest();
            if (command)
            {
                return new HandlerResult { Data = ProcessRulesRequest(message) };
            }

            return _next != null ? _next.Handle(message) : new HandlerResult();
        }

        private List<string> ProcessRulesRequest(string message)
        {
            bool messageContains(CustomRuleItem item) => item.Title.Contains(message, StringComparison.InvariantCultureIgnoreCase);
            bool messageExact(CustomRuleItem item) => item.Title.Equals(message, StringComparison.InvariantCultureIgnoreCase);

            while (_channel.TryRead(out List<CustomRuleItem>? rulesUpdate) && rulesUpdate != null)
            {
                CustomRules = rulesUpdate;
            }

            return CustomRules.Where(messageContains)
                .OrderByDescending(messageExact)
                .ThenBy(i => message.CalcLevenshteinDistance(i.Title))
                .Select(i => $"<b>{i.Title}</b>\n{i.Text}").ToList();
        }

        private static bool TryParseAsCustomRulesRequest()
        {
            return true;
        }
    }
}
