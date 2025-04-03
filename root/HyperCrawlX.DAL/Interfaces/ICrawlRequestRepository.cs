using HyperCrawlX.Models;

namespace HyperCrawlX.DAL.Interfaces
{
    public interface ICrawlRequestRepository
    {
        /// <summary>
        /// Get the status of a crawl request by its <paramref name="requestId"/>
        /// </summary>
        Task<CrawlRequestStatus> GetCrawlRequestStatus(long? requestId);

        /// <summary>
        /// Submit a new crawl request for the given <paramref name="url"/>
        /// </summary>
        Task<int> SubmitCrawlRequest(string? url);

        /// <summary>
        /// Checks if the given <paramref name="requestId"/> is valid
        /// </summary>
        Task<bool> IsValidRequestId(long requestId);
    }
}
