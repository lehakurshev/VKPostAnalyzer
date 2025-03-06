using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WebApi.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request.Path.Value;
        if (request != null && request.Contains("/api/LetterCount"))
        {
            Log.Information($"{context.Request.Path.Value}");
        }
        
        await _next(context);

        if (request != null && request.Contains("/api/LetterCount"))
        {
            Log.Information($"{context.Request} {context.Response.StatusCode}");
        }
    }
}