using Microsoft.Extensions.Logging;

namespace Services
{
    public class CrawlingService : ICrawlingService
    {
        private ILogger<CrawlingService> _logger;

        public CrawlingService(ILogger<CrawlingService> logger)
        {
            _logger = logger;
        }

        public void ProcessCrawlRequest()
        {
            // Fetch request with status 1(Queued) from the DB and start processing them.
        }
    }
}
