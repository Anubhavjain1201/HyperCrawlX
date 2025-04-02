using System.Text.Json.Serialization;

namespace HyperCrawlX.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CrawlRequestStatusEnum
    {
        Queued = 1,
        InProgress = 2,
        Completed = 3,
        Failed = 4
    }
}
