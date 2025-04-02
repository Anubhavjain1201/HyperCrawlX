using System.Text.Json.Serialization;

namespace HyperCrawlX.Models
{
    public class CrawlRequest
    {
        [JsonPropertyName("requestId")]
        public long RequestId { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("requestedDateTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime RequestedDateTime { get; set; }
    }
}
