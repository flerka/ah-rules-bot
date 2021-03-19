using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AhRulesBot.MessageProcessing
{
    internal interface IMessageValidator
    {
        public Task<bool> IsValid(Message message);
    }
}