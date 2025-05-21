using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ResumeAnalyzerAPI.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var problemDetails = new 
                {
                    Status = context.Response.StatusCode,
                    Title = _env.IsDevelopment() ? "An error occurred while processing your request." : "An internal server error occurred.",
                    Detail = _env.IsDevelopment() ? ex.ToString() : "A generic error message. Please try again later.",
                    Instance = context.Request.Path
                };

                var jsonResponse = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
