using System.Collections.Generic;

namespace AhRulesBot.MessageProcessing.Interfaces
{
    public interface IMessageHandler
    {
        public List<string> Handle(string message);
    }
}