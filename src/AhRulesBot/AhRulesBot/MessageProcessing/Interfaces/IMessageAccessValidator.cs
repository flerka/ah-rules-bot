using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AhRulesBot.MessageProcessing
{
    internal interface IMessageAccessValidator
    {
        public Task<bool> IsValid(Message message);
    }
}