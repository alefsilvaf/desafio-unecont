using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace UnecontScraping.Domain
{
    public class TransactionApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("transactions")]
        public List<Transaction> Transactions { get; set; }
        
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }
}