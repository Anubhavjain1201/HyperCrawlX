using HyperCrawlX.DAL.Interfaces;
using Microsoft.Extensions.Logging;

namespace HyperCrawlX.Services
{
    /// <summary>
    /// Service to process crawl requests.
    /// </summary>
    public class CrawlingService : ICrawlingService
    {
        private readonly ILogger<CrawlingService> _logger;
        private readonly IDbContext _dbContext; 

        public CrawlingService(
            ILogger<CrawlingService> logger,
            IDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public void ProcessCrawlRequest()
        {
            _logger.LogInformation("CrawlingService - Get the next pending crawl request from DB");

            // Get pending request from DB
            // Process the request
            // Update the results in DB
            // Update the status of the request in DB


        }
    }
}
