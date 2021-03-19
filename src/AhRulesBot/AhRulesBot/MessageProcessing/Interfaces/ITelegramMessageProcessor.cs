using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AhRulesBot.MessageProcessing
{
    public interface ITelegramMessageProcessor
    {
       public Task Process(Message message);
    }
}