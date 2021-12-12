using AhRulesBot.MessageProcessing.Interfaces;
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
            var command = TryParseAsCommand();
            if (command)
            {
                return new HandlerResult { Data = ProcessCommand(command) };
            }

            return _next != null ? _next.Handle(message) : new HandlerResult();
        }

        private List<HandlerResultData> ProcessCommand(object command)
        {
            throw new NotImplementedException();
        }

        private static bool TryParseAsCommand()
        {
            return false;
        }
    }
}
