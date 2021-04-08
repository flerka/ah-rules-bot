using AhRulesBot.MessageProcessing.Interfaces;
using AhRulesBot.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    internal class CardsMessageHandler : IMessageHandler
    {
        private IMessageHandler _next;
        private readonly List<Card> _cards;
        private readonly ILogger _logger;

        public CardsMessageHandler(
            ILogger logger,
            List<Card> cards,
            IMessageHandler next)
        {
            _logger = logger;
            _cards = cards;
            _next = next;
        }

        public HandlerResult Handle(string message)
        {
            var command = TryParseAsRulesRequest(message);
            if (command)
            {
                var processRequest = ProcessCardsRequest(message);
                if (processRequest != null && processRequest.Count != 0)
                {
                    return new HandlerResult { Data = processRequest };
                }
            }

            return _next != null ? _next.Handle(message) : new HandlerResult();
        }

        private List<string> ProcessCardsRequest(string message)
        {
            Func<Card, bool> messageContains = item => item.LocalizedName.Contains(message, StringComparison.InvariantCultureIgnoreCase);
            Func<Card, bool> messageExact = item => item.LocalizedName.Equals(message, StringComparison.InvariantCultureIgnoreCase);

            return _cards.Where(messageContains)
                .OrderByDescending(messageExact)
                .ThenBy(i => message.CalcLevenshteinDistance(i.LocalizedName))
                .Select(i => $"<b>{i.LocalizedName}</b>\n{i.Url}").ToList();
        }

        private bool TryParseAsRulesRequest(string message)
        {
            return true;
        }
    }
}
