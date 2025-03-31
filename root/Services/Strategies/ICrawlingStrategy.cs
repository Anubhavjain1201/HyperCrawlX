namespace Services.Strategies
{
    public interface ICrawlingStrategy
    {
        Task<IList<string>> ExecuteAsync(string url);
    }
}
