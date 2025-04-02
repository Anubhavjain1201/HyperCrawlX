using System.Text.Json.Serialization;

namespace HyperCrawlX.Models
{
    public class CrawlRequestStatus
    {
        [JsonPropertyName("requestId")]
        public long RequestId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("productUrls")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HashSet<string>? ProductUrls { get; set; }
    }
}
