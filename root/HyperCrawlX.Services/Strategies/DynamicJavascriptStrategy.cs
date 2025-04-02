using HyperCrawlX.Services.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Collections.Concurrent;

namespace HyperCrawlX.Services.Strategies
{
    public class DynamicJavascriptStrategy : ICrawlingStrategy
    {
        private ILogger<DynamicJavascriptStrategy> _logger;
        private const int MAX_VISIT_COUNT = 200;
        private const int MAX_SCROLL_COUNT = 5;

        public DynamicJavascriptStrategy(ILogger<DynamicJavascriptStrategy> logger)
        {
            _logger = logger;
        }

        public async Task<HashSet<string>> ExecuteAsync(string url)
        {
            _logger.LogInformation($"DynamicJavascriptStrategy - Fetching product urls for: {url}");

            IPlaywright? playwright = null;
            IBrowser? browser = null;
            IBrowserContext? context = null;
            IPage? page = null;

            try
            {
                // Initialize Headless browser using Playwright
                playwright = await Playwright.CreateAsync();
                browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = ["--disable-dev-shm-usage", 
                        "--disable-gpu", 
                        "--no-sandbox", 
                        "--disable-blink-features=AutomationControlled", 
                        "--disable-features=site-per-process", 
                        "--disable-features=IsolateOrigins,site-per-process"],
                    
                });
                context = await browser.NewContextAsync(new BrowserNewContextOptions
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36",
                    BypassCSP = true
                });
                page = await context.NewPageAsync();

                var visitedLinks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var queue = new ConcurrentQueue<string>();
                var productUrls = new HashSet<string>();
                var baseUri = new Uri(url);
                int visitCount = 0;

                queue.Enqueue(url);
                visitedLinks.Add(url);

                while (queue.TryDequeue(out var currentUrl) && visitCount < MAX_VISIT_COUNT)
                {
                    visitCount++;

                    // Navigate to the URL
                    var response = await page.GotoAsync(currentUrl, new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.DOMContentLoaded,
                    });

                    // If the response is not OK,
                    // 1. Log the error
                    // 2. Remove the URL from the list of product URLs
                    // 3. Continue to the next URL
                    if (response == null || !response.Ok)
                    {
                        _logger.LogError($"DynamicJavascriptStrategy - Error occurred while fetching {currentUrl}, statusCode: {response?.Status}");
                        productUrls.Remove(currentUrl);
                        continue;
                    }

                    // Scroll to the bottom of the page to load all the content
                    int scrollCount = 0;
                    while (scrollCount < MAX_SCROLL_COUNT)
                    {
                        var previousHeight = await page.EvaluateAsync<int>("document.body.scrollHeight");
                        await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight);");
                        await Task.Delay(1000);
                        var newHeight = await page.EvaluateAsync<int>("document.body.scrollHeight");

                        if (newHeight == previousHeight) break;
                        scrollCount++;
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

                                if (ProductPatternMatching.isProductUrl(urlString))
                                {
                                    _logger.LogInformation($"DynamicJavascriptStrategy - Found product url: {urlString}");
                                    productUrls.Add(urlString);
                                }

                                if (!visitedLinks.Contains(urlString))
                                {
                                    queue.Enqueue(urlString);
                                    visitedLinks.Add(urlString);
                                }
                            }
                        }
                    }
                }

                _logger.LogInformation($"DynamicJavascriptStrategy - Visited {visitCount} pages, Found {productUrls.Count} product URLs");
                return productUrls;
            }
            catch (Exception ex)
            {
                _logger.LogError($"DynamicJavascriptStrategy - Error occurred while crawling {url}: {ex.Message}");
                throw;
            }
            finally
            {
                // Dispose off the resources
                if (page != null)
                {
                    _logger.LogInformation("DynamicJavascriptStrategy - Closing the page");
                    await page.CloseAsync();
                }
                if(context != null)
                {
                    _logger.LogInformation("DynamicJavascriptStrategy - Closing the context");
                    await context.CloseAsync();
                }
                if (browser != null)
                {
                    _logger.LogInformation("DynamicJavascriptStrategy - Closing the browser");
                    await browser.CloseAsync();
                }
                if (playwright != null)
                {
                    _logger.LogInformation("DynamicJavascriptStrategy - Disposing off playwright");
                    playwright.Dispose();
                }
            }
        }
    }
}
