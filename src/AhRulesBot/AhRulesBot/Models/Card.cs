using System.Text.Json.Serialization;

namespace AhRulesBot.Models
{
    internal class Card
    {
        [JsonPropertyName("name")]
        public string LocalizedName { get; set; }

        [JsonPropertyName("real_name")]
        public string EnglisName { get; set; }

        [JsonPropertyName("text")]
        public string LocalizedText { get; set; }

        [JsonPropertyName("real_text")]
        public string EnglishText { get; set; }

        [JsonPropertyName("pack_code")]
        public string PackCode { get; set; }

        [JsonPropertyName("faction_code")]
        public string FactionCode { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
