using HyperCrawlX.Services.Interfaces;
using HyperCrawlX.Services.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace HyperCrawlX.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ICrawlingService, CrawlingService>();
            services.AddScoped<ICrawlRequestService, CrawlRequestService>();

            services.AddTransient<ICrawlingStrategy, DynamicJavascriptStrategy>();
            services.AddTransient<ICrawlingStrategy, HttpCrawlingStrategy>();
            return services;
        }
    }
}
