using System;
using System.Linq;

namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    internal class UnknownMessageHandler : IMessageHandler
    {
        private readonly IMessageHandler _next;
        private readonly TimeSpan TechMsgTtl = TimeSpan.FromMinutes(5);

        private const string UnknownResultMessage = "I didn't find any information";

        public UnknownMessageHandler(IMessageHandler next)
        {
            _next = next;
        }

        public HandlerResult Handle(string message)
        {
            var nextResult = _next.Handle(message);
            if (!nextResult.Data.Any())
            {
                nextResult.Data.Add(UnknownResultMessage);
                nextResult.Ttl = TechMsgTtl;
            }

            return nextResult;
        }
    }
}
