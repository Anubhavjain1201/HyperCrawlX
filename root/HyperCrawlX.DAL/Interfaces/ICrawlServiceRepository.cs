using HyperCrawlX.Models;

namespace HyperCrawlX.DAL.Interfaces
{
    public interface ICrawlServiceRepository
    {
        /// <summary>
        /// Updates the <paramref name="status"/> of the crawl request identified by <paramref name="requestId"/>
        /// </summary>
        void UpdateRequestStatus(long requestId, int status);

        /// <summary>
        /// Gets the next pending crawl reqeuest from db
        /// </summary>
        Task<CrawlRequest?> GetPendingCrawlRequest();

        /// <summary>
        /// Bulk inserts productUrls into the database for a given <paramref name="requestId"/>
        /// </summary>
        void BulkInsertUrls(long requestId, List<string> urls);
    }
}
