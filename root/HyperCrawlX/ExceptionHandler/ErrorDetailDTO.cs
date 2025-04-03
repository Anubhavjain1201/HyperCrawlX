using System.Text.Json.Serialization;

namespace HyperCrawlX.ExceptionHandler
{
    public class ErrorDetailDTO
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
    }
}
