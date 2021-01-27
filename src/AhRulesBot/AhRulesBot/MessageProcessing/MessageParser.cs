using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AhRulesBot.MessageProcessing
{
    internal class MessageParser : IMessageParser
    {
        private readonly IMessageAccessValidator _messageAcccessValidator; 
        public async Task Parse (Message message)
        {
            if (await _messageAcccessValidator.IsValid(message))
            {
                return;
            }
        }
    }
}
