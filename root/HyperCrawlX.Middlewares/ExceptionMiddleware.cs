using HyperCrawlX.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace HyperCrawlX.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (CustomException ex)
            {
                _logger.LogError($"Middleware - Custom Exception occurred - {ex.StackTrace}");
                await HandleExceptionAsync(context, ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Middleware - Exception occurred while processing the request - {ex.Message}");
                await HandleExceptionAsync(context, (int)HttpStatusCode.InternalServerError, "An unexpected error occurred!");
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var exception = new ErrorDetailDTO()
            {
                StatusCode = statusCode,
                ErrorMessage = message
            };
            string errorResponse = JsonSerializer.Serialize(exception);

            await context.Response.WriteAsync(errorResponse);
        }
    }
}
