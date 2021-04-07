using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AhRulesBot.BotRequestsProcessing
{
    internal interface ITelegramMessageProcessor
    {
        public Task Process(Message message, CancellationToken cancellationToken);
    }
}