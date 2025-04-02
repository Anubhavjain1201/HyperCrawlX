using HyperCrawlX.Models.Enums;
using System.Text.Json.Serialization;

namespace HyperCrawlX.Models
{
    public class CrawlRequestStatus
    {
        [JsonPropertyName("requestId")]
        public long RequestId { get; set; }

        [JsonPropertyName("status")]
        public CrawlRequestStatusEnum Status { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("productUrlsCount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? ProductUrlsCount { get; set; }

        [JsonPropertyName("productUrls")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? ProductUrls { get; set; }
    }
}
