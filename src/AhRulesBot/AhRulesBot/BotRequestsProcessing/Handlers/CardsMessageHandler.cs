using AhRulesBot.Infrastructure;
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
        private readonly AppConfig _config;

        public CardsMessageHandler(
            List<Card> cards,
            AppConfig config,
            IMessageHandler next)
        {
            _cards = cards;
            _next = next;
            _config = config;
        }

        public HandlerResult Handle(string message)
        {
            var command = TryParseAsCardsRequest();
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

        private List<HandlerResultData> ProcessCardsRequest(string message)
        {
            bool messageContains(Card item) => item.LocalizedName.Contains(message, StringComparison.InvariantCultureIgnoreCase);
            bool messageExact(Card item) => item.LocalizedName.Equals(message, StringComparison.InvariantCultureIgnoreCase);

            var cards = _cards.Where(messageContains)
                .OrderByDescending(messageExact)
                .ThenBy(i => message.CalcLevenshteinDistance(i.LocalizedName))
                .ThenBy(i => i.LocalizedName == i.EnglisName)
                .ToList();

            if (cards.Count == 0)
            {
                return new List<HandlerResultData>();
            }

            var otherCardsText = "";
            if (cards.Count > 1)
            {
                otherCardsText = "\nДругие карты: " + String.Join(", ", cards.Skip(1).Select(item => $"<a href=\"{item.Url}\">{item.LocalizedName}</a>"));
            }
            var data = new HandlerResultData { Text = $"<b>{cards[0].LocalizedName}</b>\n{cards[0].Url}{otherCardsText}" };
            if (!IsSpoilerRequired(cards[0]))
            {
                return new List<HandlerResultData> { data };
            }

            var stickerData = new HandlerResultData { TelegramStickerId = GetSpoilerId(cards[0].Code) };
            return new List<HandlerResultData> { stickerData, data, stickerData }; ;
        }

        private static bool TryParseAsCardsRequest()
        {
            return true;
        }

        private string GetSpoilerId(string code)
        {
            var shortCode = code.Substring(0, 2);
            return shortCode switch
            {
                "01" => _config.NightAzStSpoilerIdTg,
                "02" => _config.DunwichStSpoilerIdTg,
                "03" => _config.CarcosaStSpoilerIdTg,
                "04" => _config.CircleUndoneStSpoilerIdTg,
                _ => _config.NotLocalStSpoilerIdTg
            };
        }

        private bool IsSpoilerRequired(Card card)
        {
            if (card.FactionCode == "mythos")
            {
                return true;
            }

            return !(card.Code.StartsWith("01") || card.Code.StartsWith("02") || card.Code.StartsWith("03") || card.Code.StartsWith("04"));
        }
    }
}
