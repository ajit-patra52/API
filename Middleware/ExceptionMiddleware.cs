using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPI6.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Call the next delegate/middleware in the pipeline
                 await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An unhandled exception has occurred while processing the request.");              
                await HandleExceptionAsync(httpContext, ex);
            }            
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "An unexpected error occurred. Please try again later.",
                Details = exception.Message // Optional: Include exception details in development
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
   
}
