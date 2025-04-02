using HyperCrawlX.Models;

namespace HyperCrawlX.DAL.Repositories
{
    public interface ICrawlRequestRepository
    {
        Task<CrawlRequestStatus> GetCrawlRequestStatus(long? requestId);

        Task<int> SubmitCrawlRequest(string? url);

        Task<bool> IsValidRequestId(long requestId);
    }
}
