namespace AhRulesBot.BotRequestsProcessing.Handlers
{
    internal interface IMessageHandler
    {
        public HandlerResult Handle(string message);
    }
}