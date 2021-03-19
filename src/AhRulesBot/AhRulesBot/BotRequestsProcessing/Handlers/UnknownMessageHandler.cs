using AhRulesBot.MessageProcessing.Interfaces;
using System;
using System.Linq;

namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    public class UnknownMessageHandler : IMessageHandler
    {
        private IMessageHandler _next;
        private const string UnknownResultMessage = "I didn't find any information";
        private readonly TimeSpan TechMsgTtl = TimeSpan.FromMinutes(5);

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
