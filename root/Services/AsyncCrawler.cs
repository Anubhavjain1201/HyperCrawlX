using Microsoft.Extensions.Hosting;

namespace Services
{
    public class AsyncCrawler : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
