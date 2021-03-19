using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AhRulesBot.BotRequestsProcessing.Interfaces
{
    internal interface IMessageValidator
    {
        public Task<bool> IsValid(Message message);
    }
}