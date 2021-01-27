using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AhRulesBot.MessageProcessing
{
    internal interface IMessageParser
    {
        public Task Parse(Message message);
    }
}