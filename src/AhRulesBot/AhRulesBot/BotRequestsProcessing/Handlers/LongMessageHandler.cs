using AhRulesBot.MessageProcessing.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    public class LongMessageHandler : IMessageHandler
    {
        private IMessageHandler _next;

        public LongMessageHandler(IMessageHandler next)
        {
            _next = next;
        }

        public HandlerResult Handle(string message)
        {
            var nextResult = _next.Handle(message);

            var concatMessage = string.Join("\n\n\n\n", nextResult.Data);
            nextResult.Data = concatMessage.Length <= 4000 ?
                new List<string>() { concatMessage } :
                nextResult.Data.Take(2).ToList();

            return nextResult;
        }
    }
}
