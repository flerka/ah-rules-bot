using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AhRulesBot.MessageProcessing
{
    internal class MessageAccessValidator : IMessageAccessValidator
    {
        public async Task<bool> IsValid(Message message)
        {
            return false;
        }
    }
}
