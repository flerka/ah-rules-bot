using AhRulesBot.BotRequestsProcessing.Validators;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AhRulesBot.BotRequestsProcessing
{
    internal interface IMessageValidator
    {
        public Task<ValidatorResult> IsValid(Message message);
    }
}