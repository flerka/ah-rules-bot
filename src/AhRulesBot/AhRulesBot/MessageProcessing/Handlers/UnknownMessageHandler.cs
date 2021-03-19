using AhRulesBot.MessageProcessing.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AhRulesBot.MessageProcessing
{
    public class UnknownMessageHandler : IMessageHandler
    {
        private IMessageHandler _next;
        private const string UnknownResultMessage = "I didn't find any information";

        public UnknownMessageHandler(IMessageHandler next)
        {
            _next = next;
        }

        public List<string> Handle(string message)
        {
            var nextResult = _next.Handle(message);
            if (!nextResult.Any())
            {
                return new List<string>() { UnknownResultMessage };
            }

            return nextResult;
        }
    }
}
