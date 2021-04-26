
namespace AhRulesBot.Infrastructure
{
    public class AppConfig
    {
        public long TestChatId { get; set; }
        public long AHChatId { get; set; }
        public long BotAdminId { get; set; }

        public string ApiKey { get; set; } = string.Empty;
        public string BotName { get; set; } =  string.Empty;

        public string GoogleCredFilePath { get; set; } = string.Empty;
        public string GoogleFileId { get; set; } = string.Empty;

        public string RulesFilePath { get; set; } = string.Empty;
        public string CardsFilePath { get; set; } = string.Empty;
    }
}