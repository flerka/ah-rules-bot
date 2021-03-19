using AhRulesBot.MessageProcessing.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AhRulesBot.MessageProcessing
{
    public class LongMessageHandler : IMessageHandler
    {
        private IMessageHandler _next;

        public LongMessageHandler(IMessageHandler next)
        {
            _next = next;
        }

        public List<string> Handle(string message)
        {
            var nextResult = _next.Handle(message);

            var concatMessage = string.Join("\n\n\n\n", nextResult);
            return concatMessage.Length <= 4000 ? 
                new List<string>() { concatMessage } : 
                nextResult.Take(2).ToList();
        }
    }
}
