﻿using HyperCrawlX.DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HyperCrawlX.DAL
{
    public static class DbExtensions
    {
        public static IServiceCollection RegisterDbDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IDbConnectionManager, DbConnectionManager>();
            services.AddScoped<IDbContext, DbContext>();
            return services;
        }
    }
}
