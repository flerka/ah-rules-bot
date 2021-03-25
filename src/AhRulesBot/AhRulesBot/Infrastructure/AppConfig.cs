
namespace AhRulesBot.Infrastructure
{
    public class AppConfig
    {
        public long TestChatId { get; set; }
        public long AHChatId { get; set; }
        public long BotAdminId { get; set; }

        public string ApiKey { get; set; }
        public string BotName { get; set; }

        public string GoogleCredFilePath { get; set; }
        public string GoogleFileId { get; set; }

        public string RulesFilePath { get; set; }
        public string CardsFilePath { get; set; }
    }
}