using Services;

namespace HyperCrawlX.BackgroundWorkers
{
    public class AsyncCrawler : BackgroundService
    {
        private readonly ILogger<AsyncCrawler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private bool IsCurrentlyProcessing = false;

        public AsyncCrawler(ILogger<AsyncCrawler> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"AsyncCrawler - Background service running at: {DateTimeOffset.Now}");
                if (!IsCurrentlyProcessing)
                {
                    _logger.LogInformation($"AsyncCrawler - No request is currently getting processed!");
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            ICrawlingService crawlingService = scope.ServiceProvider.GetRequiredService<ICrawlingService>();
                            crawlingService.ProcessCrawlRequest();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"AsyncCrawler - Exception occurred while processing the request: {ex.Message}");
                        // update the request status to failed.
                    }
                    IsCurrentlyProcessing = false;
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
