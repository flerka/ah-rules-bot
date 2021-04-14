using AhRulesBot.MessageProcessing.Interfaces;
using AhRulesBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    internal class CardsMessageHandler : IMessageHandler
    {
        private readonly IMessageHandler _next;
        private readonly List<Card> _cards;

        public CardsMessageHandler(
            List<Card> cards,
            IMessageHandler next)
        {
            _cards = cards;
            _next = next;
        }

        public HandlerResult Handle(string message)
        {
            var command = TryParseAsRulesRequest();
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
            bool messageContains(Card item) => item.LocalizedName.Contains(message, StringComparison.InvariantCultureIgnoreCase);
            bool messageExact(Card item) => item.LocalizedName.Equals(message, StringComparison.InvariantCultureIgnoreCase);

            return _cards.Where(messageContains)
                .OrderByDescending(messageExact)
                .ThenBy(i => message.CalcLevenshteinDistance(i.LocalizedName))
                .Select(i => $"<b>{i.LocalizedName}</b>\n{i.Url}").ToList();
        }

        private static bool TryParseAsRulesRequest()
        {
            return true;
        }
    }
}
