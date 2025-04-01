using HyperCrawlX.Services.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Collections.Concurrent;

namespace HyperCrawlX.Services.Strategies
{
    public class DynamicJavascriptStrategy : ICrawlingStrategy
    {
        private ILogger<DynamicJavascriptStrategy> _logger;

        public DynamicJavascriptStrategy(ILogger<DynamicJavascriptStrategy> logger)
        {
            _logger = logger;
        }

        public async Task<IList<string>> ExecuteAsync(string url)
        {
            _logger.LogInformation($"DynamicJavascriptStrategy - Fetching product urls for: {url}");

            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = ["--disable-dev-shm-usage", "--disable-gpu", "--no-sandbox"]
            });
            var page = await browser.NewPageAsync();

            try
            {
                var visitedLinks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var queue = new ConcurrentQueue<string>();
                var productUrls = new List<string>();
                var baseUri = new Uri(url);

                queue.Enqueue(url);
                visitedLinks.Add(url);

                // TODO: Also add maximum page visit limit
                while (queue.TryDequeue(out var currentUrl))
                {
                    // Check if the current URL is a product URL
                    if (ProductPatternMatching.isProductUrl(currentUrl))
                    {
                        _logger.LogInformation($"HttpCrawlingStrategy - Found product url: {url}");
                        productUrls.Add(url);
                    }

                    // Navigate to the URL
                    var response = await page.GotoAsync(currentUrl, new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.NetworkIdle,
                    });

                    if (response == null || !response.Ok)
                    {
                        _logger.LogError($"DynamicJavascriptStrategy - Error occurred while fetching {url}");
                        throw new Exception($"Error occurred while fetching content from: {url}");
                    }

                    // Scroll to the bottom of the page to load all the content
                    while (true)
                    {
                        var previousHeight = await page.EvaluateAsync<int>("document.body.scrollHeight");
                        await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight);");
                        await Task.Delay(1000);
                        var newHeight = await page.EvaluateAsync<int>("document.body.scrollHeight");

                        if (newHeight == previousHeight) break;
                    }

                    // Get all links from the page
                    var discoveredLinks = await page.EvaluateAsync<string[]>(@"
                    Array.from(document.querySelectorAll('a[href]'))
                        .map(a => a.href)
                    ");

                    // Add the discovered links to the queue
                    foreach (var link in discoveredLinks ?? Array.Empty<string>())
                    {
                        if (Uri.TryCreate(baseUri, link, out var absoluteUri))
                        {
                            // Only consider absolute links from the same domain
                            if (absoluteUri.Host == baseUri.Host)
                            {
                                var urlString = absoluteUri.ToString();
                                if (!visitedLinks.Contains(urlString))
                                {
                                    queue.Enqueue(urlString);
                                    visitedLinks.Add(urlString);
                                }
                            }
                        }
                    }
                }

                _logger.LogInformation($"DynamicJavascriptStrategy - Visited {visitedLinks.Count} pages, Found {productUrls.Count} product URLs");
                return productUrls;
            }
            catch (Exception ex)
            {
                _logger.LogError($"DynamicJavascriptStrategy - Error occurred while crawling {url}: {ex.Message}");
                throw;
            }
            finally
            {
                if (page != null)
                {
                    await page.CloseAsync();
                }
                if (browser != null)
                {
                    await browser.CloseAsync();
                }
                if (playwright != null)
                {
                    playwright.Dispose();
                }
            }
        }
    }
}
