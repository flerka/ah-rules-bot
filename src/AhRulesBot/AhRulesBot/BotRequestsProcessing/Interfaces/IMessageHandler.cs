using AhRulesBot.BotRequestsProcessing;
using System.Collections.Generic;

namespace AhRulesBot.MessageProcessing.Interfaces
{
    public interface IMessageHandler
    {
        public HandlerResult Handle(string message);
    }
}