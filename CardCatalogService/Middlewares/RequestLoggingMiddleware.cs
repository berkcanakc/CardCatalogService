using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CardCatalogService.API.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var method = context.Request.Method;
            var path = context.Request.Path;

            _logger.LogInformation("→ HTTP {Method} {Path}", method, path);

            await _next(context); // isteği gönder

            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            _logger.LogInformation("← {StatusCode} ({Duration}ms)", statusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
