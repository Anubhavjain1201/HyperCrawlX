using HyperCrawlX.Models;

namespace HyperCrawlX.DAL.Interfaces
{
    public interface ICrawlRequestRepository
    {
        /// <summary>
        /// 
        /// </summary>
        Task<CrawlRequestStatus> GetCrawlRequestStatus(long? requestId);

        Task<int> SubmitCrawlRequest(string? url);

        Task<bool> IsValidRequestId(long requestId);
    }
}
