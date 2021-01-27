using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AhRulesBot.MessageProcessing
{
    public interface IMessageHandler
    {
       public Task Handle(Message message);
    }
}