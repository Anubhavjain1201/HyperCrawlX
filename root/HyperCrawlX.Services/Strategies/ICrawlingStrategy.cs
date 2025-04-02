namespace HyperCrawlX.Services.Strategies
{
    public interface ICrawlingStrategy
    {
        Task<HashSet<string>> ExecuteAsync(string url);
    }
}
