using HtmlAgilityPack;
using HyperCrawlX.Services.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace HyperCrawlX.Services.Strategies
{
    public class HttpCrawlingStrategy : ICrawlingStrategy
    {
        private ILogger<HttpCrawlingStrategy> _logger;
        private const int MAX_VISIT_COUNT = 200;

        public HttpCrawlingStrategy(ILogger<HttpCrawlingStrategy> logger)
        {
            _logger = logger;
        }

        public async Task<HashSet<string>> ExecuteAsync(string url)
        {
            try
            {
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

                    HttpResponseMessage? response = null;
                    string html;
                    using (var httpClient = new HttpClient())
                    {
                        response = await httpClient.GetAsync(currentUrl);
                        html = await response.Content.ReadAsStringAsync();
                    }

                    // Parse the HTML content
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);

                    var discoveredLinks = new HashSet<string>();
                    var anchorNodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
                    if (anchorNodes != null)
                    {
                        foreach (var anchorNode in anchorNodes)
                        {
                            // Get the href attribute value
                            var href = anchorNode.GetAttributeValue("href", "");
                            if (string.IsNullOrWhiteSpace(href))
                                continue;

                            if (Uri.TryCreate(baseUri, href, out var absoluteUri))
                            {
                                // Only consider absolute links from the same domain
                                if (absoluteUri.Host == baseUri.Host)
                                {
                                    var urlString = absoluteUri.ToString();

                                    if (ProductPatternMatching.isProductUrl(urlString))
                                    {
                                        _logger.LogInformation($"HttpCrawlingStrategy - Found product url: {urlString}");
                                        productUrls.Add(urlString);
                                    }

                                    if (!visitedLinks.Contains(urlString))
                                    {
                                        // Add link to the queue and to the visited set
                                        queue.Enqueue(urlString);
                                        visitedLinks.Add(urlString);
                                    }
                                }
                            }
                        }
                    }
                }

                _logger.LogInformation($"HttpCrawlingStrategy - Visited {visitCount} pages, Found {productUrls.Count} product URLs");
                return productUrls;
            }
            catch (Exception ex)
            {
                _logger.LogError($"HttpCrawlingStrategy - Error occurred while crawling {url}: {ex.Message}");
                throw;
            }
        }
    }
}
