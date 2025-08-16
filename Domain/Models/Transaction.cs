using System.Text.Json.Serialization;

namespace UnecontScraping.Domain
{
    public class Transaction
    {
        [JsonPropertyName("timestamp")]
        public string? Time { get; set; }
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [JsonPropertyName("gold")]
        public decimal? Value { get; set; }
    }
}