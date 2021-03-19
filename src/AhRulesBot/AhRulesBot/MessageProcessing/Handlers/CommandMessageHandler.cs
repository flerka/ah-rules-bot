using AhRulesBot.MessageProcessing.Interfaces;
using System;
using System.Collections.Generic;

namespace AhRulesBot.MessageProcessing
{
    public class CommandMessageHandler : IMessageHandler
    {
        private IMessageHandler _next;

        public CommandMessageHandler(IMessageHandler next)
        {
            _next = next;
        }

        public List<string> Handle(string message)
        {
            var command = TryParseAsCommand(message);
            if (command)
            {
                return ProcessCommand(command);
            }

            return _next != null ? _next.Handle(message) : new List<string>();
        }

        private List<string> ProcessCommand(object command)
        {
            throw new NotImplementedException();
        }

        private bool TryParseAsCommand(string message)
        {
            return false;
        }
    }
}
