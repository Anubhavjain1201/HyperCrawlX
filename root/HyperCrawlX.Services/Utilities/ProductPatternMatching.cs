using System.Text.RegularExpressions;

namespace HyperCrawlX.Services.Utilities
{
    public static class ProductPatternMatching
    {
        private readonly static List<string> _productPatterns = new List<string>
        {
            @"/product[s]?/", @"/item[s]?/", @"/p/", @"/pd/", @"/shop/",
            @"/buy/", @"/-pr-", @"/good[s]?/", @"/detail/", @"/prod[s]?-",
            @"/prod[s]?/", @"/detail[s]?/", @"/dp/", @"/B[A-Z0-9]{9,10}",
            @"(product|item|catalog).*?(id|sku|code)=", @"/collection.*?/products/"
        };

        private readonly static List<string> _nonProductPatterns = new List<string>
        {
            @"/cart", @"/checkout", @"/login", @"/register", @"/account",
            @"/search", @"/wishlist", @"/category", @"/collection[s]?$",
            @"/support", @"/faq", @"/help", @"/about", @"/contact",
            @"/blog", @"/news", @"/article[s]?", @"/static/", @"/asset[s]?/",
            @"/term[s]?", @"/policy", @"/privacy", @"/shipping", @"/return[s]?"
        };

        /// <summary>
        /// Checks if the <paramref name="url"/> is for a product
        /// </summary>
        /// <param name="url"></param>
        /// <returns>true, if the <paramref name="url"/> is a product url, else false</returns>
        public static bool isProductUrl(string url)
        {
            // Check if the URL matches any product pattern
            var isProductUrl = _productPatterns.Any(pattern => Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase));

            // Check if the URL matches any non-product pattern
            var isNonProductUrl = _nonProductPatterns.Any(pattern => Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase));

            var uri = new Uri(url);
            bool hasProductQueryParams = false;

            if (!string.IsNullOrEmpty(uri.Query))
            {
                var queryParams = uri.Query.TrimStart('?').Split('&');
                hasProductQueryParams = queryParams.Any(param =>
                    param.StartsWith("id=") ||
                    param.StartsWith("pid=") ||
                    param.StartsWith("product=") ||
                    param.StartsWith("sku="));
            }

            return (isProductUrl || hasProductQueryParams) && !isNonProductUrl;
        }
    }
}
