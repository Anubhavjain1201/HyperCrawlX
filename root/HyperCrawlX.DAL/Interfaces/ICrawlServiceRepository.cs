using HyperCrawlX.Models;

namespace HyperCrawlX.DAL.Interfaces
{
    public interface ICrawlServiceRepository
    {
        void UpdateRequestStatus(long requestId, int status);

        Task<CrawlRequest?> GetPendingCrawlRequest();

        void BulkInsertUrls(long requestId, List<string> urls);
    }
}
