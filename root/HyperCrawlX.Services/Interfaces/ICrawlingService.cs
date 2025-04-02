namespace HyperCrawlX.Services.Interfaces
{
    public interface ICrawlingService
    {
        /// <summary>
        /// Fetches and processes the crawl requests from the database.
        /// </summary>
        void FindAndProcessCrawlRequest();
    }
}
