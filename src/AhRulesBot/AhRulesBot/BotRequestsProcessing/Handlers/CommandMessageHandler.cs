using System;
using System.Collections.Generic;

namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    internal class CommandMessageHandler : IMessageHandler
    {
        private readonly IMessageHandler _next;

        public CommandMessageHandler(IMessageHandler next)
        {
            _next = next;
        }

        public HandlerResult Handle(string message)
        {
            var command = TryParseAsCommand(message);
            if (command)
            {
                return new HandlerResult { Data = ProcessCommand(command) };
            }

            return _next != null ? _next.Handle(message) : new HandlerResult();
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
