
using System.Text.Json;

namespace OrderManagement.WebAPI.Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the full exception, including its stack trace.
                _logger.LogError(
                    ex,
                    "An unhandled exception occurred while processing {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                // Do not attempt to replace an already-started response.
                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode =
                    StatusCodes.Status500InternalServerError;

                context.Response.ContentType = "application/json";

                var response = new
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "An unexpected error occurred.",
                    traceId = context.TraceIdentifier,
                    details = ex.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

    public static class UseExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
