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

        public async Task<IList<string>> ExecuteAsync(string url)
        {
            try
            {
                var visitedLinks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var queue = new ConcurrentQueue<string>();
                var productUrls = new List<string>();

                queue.Enqueue(url);
                visitedLinks.Add(url);

                // TODO: Also add maximum page visit limit
                while (queue.Count > 0)
                {
                    queue.TryDequeue(out var currentUrl);

                    HttpResponseMessage? response = null;
                    string html;
                    using (var httpClient = new HttpClient())
                    {
                        response = await httpClient.GetAsync(currentUrl);
                        html = await response.Content.ReadAsStringAsync();
                    }

                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);

                    var baseUri = new Uri(url);

                    var discoveredLinks = new HashSet<string>();
                    var anchorNodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
                    if (anchorNodes != null)
                    {
                        foreach (var anchorNode in anchorNodes)
                        {
                            var href = anchorNode.GetAttributeValue("href", "");
                            if (string.IsNullOrWhiteSpace(href))
                                continue;

                            if (Uri.TryCreate(baseUri, href, out var absoluteUri))
                            {
                                if (absoluteUri.Host == baseUri.Host)
                                    discoveredLinks.Add(absoluteUri.ToString());
                            }
                        }
                    }

                    if (ProductPatternMatching.isProductUrl(url))
                    {
                        _logger.LogInformation($"HttpCrawlingStrategy - Found product url: {url}");
                        productUrls.Add(url);
                    }

                    foreach (var link in discoveredLinks ?? Enumerable.Empty<string>())
                    {
                        if (!visitedLinks.Contains(link))
                            queue.Enqueue(link);
                    }
                }

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
