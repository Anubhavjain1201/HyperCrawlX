using HtmlAgilityPack;
using HyperCrawlX.Services.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace HyperCrawlX.Services.Strategies
{
    public class HttpCrawlingStrategy : ICrawlingStrategy
    {
        private ILogger<HttpCrawlingStrategy> _logger;

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

                queue.Enqueue(url);
                visitedLinks.Add(url);

                // TODO: Also add maximum page visit limit
                while (queue.TryDequeue(out var currentUrl))
                {
                    if (ProductPatternMatching.isProductUrl(currentUrl))
                    {
                        _logger.LogInformation($"HttpCrawlingStrategy - Found product url: {currentUrl}");
                        productUrls.Add(currentUrl);
                    }

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

                    // Get the base URI
                    var baseUri = new Uri(url);

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

                            // Only consider absolute links from the same domain
                            if (Uri.TryCreate(baseUri, href, out var absoluteUri))
                            {
                                if (absoluteUri.Host == baseUri.Host)
                                    discoveredLinks.Add(absoluteUri.ToString());
                            }
                        }
                    }
                        
                    foreach (var link in discoveredLinks ?? Enumerable.Empty<string>())
                    {
                        if (!visitedLinks.Contains(link))
                        {
                            // Add link to the queue and to the visited set
                            queue.Enqueue(link);
                            visitedLinks.Add(link);
                        }
                    }
                }

                _logger.LogInformation($"HttpCrawlingStrategy - Visited {visitedLinks.Count} pages, Found {productUrls.Count} product URLs");
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
