using AhRulesBot.MessageProcessing.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    internal class LongMessageHandler : IMessageHandler
    {
        private readonly IMessageHandler _next;

        public LongMessageHandler(IMessageHandler next)
        {
            _next = next;
        }

        public HandlerResult Handle(string message)
        {
            var nextResult = _next.Handle(message);
            var textItems = nextResult.Data.Select(i => i.Text);
            var concatMessage = string.Join("\n\n\n\n", textItems);
            nextResult.Data = nextResult.Data.All(i => i.TelegramImageUrl == null && i.TelegramStickerId == null) && concatMessage.Length <= 4000 ?
                new List<HandlerResultData> { new HandlerResultData { Text = concatMessage } } :
                nextResult.Data.Take(3).ToList();

            return nextResult;
        }
    }
}
