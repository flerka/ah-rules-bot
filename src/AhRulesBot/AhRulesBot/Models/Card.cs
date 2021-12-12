using System.Text.Json.Serialization;

namespace AhRulesBot.Models
{
    internal class Card
    {
        [JsonPropertyName("name")]
        public string LocalizedName { get; set; } = string.Empty;

        [JsonPropertyName("real_name")]
        public string EnglisName { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string LocalizedText { get; set; } = string.Empty;

        [JsonPropertyName("real_text")]
        public string EnglishText { get; set; } = string.Empty;

        [JsonPropertyName("pack_code")]
        public string PackCode { get; set; } = string.Empty;

        [JsonPropertyName("faction_code")]
        public string FactionCode { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
    }
}
