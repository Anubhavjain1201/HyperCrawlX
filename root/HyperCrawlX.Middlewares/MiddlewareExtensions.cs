using Microsoft.AspNetCore.Builder;

namespace HyperCrawlX.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static void AddCustomMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
