
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

        public string NightAzStSpoilerIdTg { get; set; } = string.Empty;
        public string DunwichStSpoilerIdTg { get; set; } = string.Empty;
        public string CarcosaStSpoilerIdTg { get; set; } = string.Empty;
        public string ForgAgeStSpoilerIdTg { get; set; } = string.Empty;
        public string CircleUndoneStSpoilerIdTg { get; set; } = string.Empty;
        public string StandAloneStSpoilerIdTg { get; set; } = string.Empty;
        public string NotLocalStSpoilerIdTg { get; set; } = string.Empty;
        public string NotNotLocalStSpoilerIdTg { get; set; } = string.Empty;
    }
}