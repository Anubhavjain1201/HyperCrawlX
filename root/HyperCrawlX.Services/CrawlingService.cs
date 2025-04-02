using HyperCrawlX.DAL.Interfaces;
using HyperCrawlX.Models;
using HyperCrawlX.Models.Enums;
using HyperCrawlX.Services.Interfaces;
using HyperCrawlX.Services.Strategies;
using Microsoft.Extensions.Logging;

namespace HyperCrawlX.Services
{
    /// <summary>
    /// Service to process crawl requests.
    /// </summary>
    public class CrawlingService : ICrawlingService
    {
        private readonly ILogger<CrawlingService> _logger;
        private readonly ICrawlServiceRepository _crawlServiceRepository;
        private readonly IEnumerable<ICrawlingStrategy> _crawlingStrategies;

        public CrawlingService(
            ILogger<CrawlingService> logger,
            ICrawlServiceRepository crawlServiceRepository,
            IEnumerable<ICrawlingStrategy> crawlingStrategies)
        {
            _logger = logger;
            _crawlServiceRepository = crawlServiceRepository;
            _crawlingStrategies = crawlingStrategies;
        }

        public void FindAndProcessCrawlRequest()
        {
            // Get pending request from DB
            CrawlRequest? request = GetPendingCrawlRequest();
            if (request != null)
            {
                // Process the request
                _logger.LogInformation($"CrawlingService - Starting processing of the request: {request.RequestId}");
                ProcessCrawlRequest(request);
                _logger.LogInformation($"CrawlingService - Completed processing of the request: {request.RequestId}");
            }
        }

        private void ProcessCrawlRequest(CrawlRequest request)
        {
            try
            {
                // Fetch product URLs
                var productUrls = new HashSet<string>();
                foreach (var strategy in _crawlingStrategies)
                {
                    var products = strategy.ExecuteAsync(request.Url!).Result;
                    productUrls.UnionWith(products);
                }
                // Add Product URLs to DB
                InsertProductUrls(request, productUrls.ToList());
                // Update the request status to Completed
                UpdateRequestStatus(request, CrawlRequestStatusEnum.Completed);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CrawlingService - Failed to process crawl request: {request.RequestId} for url: {request.Url}: {ex.Message}");
                UpdateRequestStatus(request, CrawlRequestStatusEnum.Failed);
            }
        }

        private void InsertProductUrls(CrawlRequest request, List<string> productUrls)
        {

        }

        /// <summary>
        /// Update the <paramref name="status"/> of the <paramref name="request"/> in DB.
        /// </summary>
        private void UpdateRequestStatus(CrawlRequest request, CrawlRequestStatusEnum status)
        {
            _crawlServiceRepository.UpdateRequestStatus(request.RequestId, (int)status);
            return;
        }

        /// <summary>
        /// Fetches the next pending crawl request from DB.
        /// </summary>
        private CrawlRequest? GetPendingCrawlRequest()
        {
            // Get pending request from DB
            var request = _crawlServiceRepository.GetPendingCrawlRequest().Result;
            if (request != null)
            {
                // Update the request status to InProgress
                UpdateRequestStatus(request, CrawlRequestStatusEnum.InProgress);
            }
            return request;
        }
    }
}
