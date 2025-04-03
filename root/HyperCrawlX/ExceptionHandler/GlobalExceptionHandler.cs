using HyperCrawlX.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace HyperCrawlX.ExceptionHandler
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            if(exception != null && exception is CustomException)
            {
                var customException = exception as CustomException;
                _logger.LogError($"Middleware - Custom Exception occurred - {customException!.ErrorMessage}");
                await HandleExceptionAsync(httpContext, customException!.StatusCode, customException!.ErrorMessage);
            }
            else if( exception != null && exception is Exception)
            {
                _logger.LogError($"Middleware - Exception occurred while processing the request - {exception.Message}");
                await HandleExceptionAsync(httpContext, (int)HttpStatusCode.InternalServerError, "An unexpected error occurred!");
            }

            return true;
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
