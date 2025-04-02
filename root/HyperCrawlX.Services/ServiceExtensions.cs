using HyperCrawlX.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HyperCrawlX.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ICrawlingService, CrawlingService>();
            services.AddScoped<ICrawlRequestService, CrawlRequestService>();
            return services;
        }
    }
}
