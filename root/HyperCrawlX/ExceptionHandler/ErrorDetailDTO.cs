using System.Text.Json.Serialization;

namespace HyperCrawlX.Middlewares
{
    public class ErrorDetailDTO
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
    }
}
