using HyperCrawlX.Models;

namespace HyperCrawlX.Services.Interfaces
{
    public interface ICrawlRequestService
    {
        /// <summary>
        /// Fetches the status of the crawl request identified by <paramref name="requestId"/>
        /// </summary>
        Task<CrawlRequestStatus> GetCrawlRequestStatus(long? requestId);

        /// <summary>
        /// Submit a new crawl request for the given <paramref name="url"/>
        /// </summary>
        Task<CrawlRequest> SubmitCrawlRequest(string? url);
    }
}
