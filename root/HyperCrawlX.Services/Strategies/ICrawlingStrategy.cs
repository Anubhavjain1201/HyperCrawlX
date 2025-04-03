namespace HyperCrawlX.Services.Strategies
{
    public interface ICrawlingStrategy
    {
        /// <summary>
        /// Execute the crawling strategy.
        /// </summary>
        Task<HashSet<string>> ExecuteAsync(string url);
    }
}
