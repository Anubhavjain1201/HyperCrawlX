using HyperCrawlX.DAL.Interfaces;
using HyperCrawlX.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HyperCrawlX.DAL
{
    public static class DALExtensions
    {
        public static IServiceCollection RegisterDbDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IDbConnectionManager, DbConnectionManager>();
            services.AddScoped<ICrawlRequestRepository, CrawlRequestRepository>();
            services.AddScoped<ICrawlServiceRepository, CrawlServiceRepository>();
            return services;
        }
    }
}
