using AhRulesBot.BotRequestsProcessing;

namespace AhRulesBot.MessageProcessing.Interfaces
{
    internal interface IMessageHandler
    {
        public HandlerResult Handle(string message);
    }
}