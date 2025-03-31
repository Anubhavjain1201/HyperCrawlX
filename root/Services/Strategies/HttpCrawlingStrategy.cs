using Microsoft.Extensions.Logging;

namespace Services.Strategies
{
    public class HttpCrawlingStrategy : ICrawlingStrategy
    {
        private ILogger<HttpCrawlingStrategy> _logger;

        public HttpCrawlingStrategy(ILogger<HttpCrawlingStrategy> logger)
        {
            _logger = logger;
        }

        public async Task<IList<string>> ExecuteAsync(string url)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"HttpCrawlingStrategy - Error occurred while crawling {url}: {ex.Message}");
                throw;
            }
        }
    }
}
