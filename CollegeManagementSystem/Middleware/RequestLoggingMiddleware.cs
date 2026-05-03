using System.Diagnostics;
using System.Security.Claims;

namespace CollegeManagementSystem.Middleware;

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

        // Get user identity
        var user = context.User;
        string userRole = user.Identity.IsAuthenticated
            ? user.FindFirst(ClaimTypes.Role)?.Value ?? "No Role"
            : "Anonymous";

        // Capture response status code (without modifying response body)
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            // Important: Seek to beginning and copy to original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            
            // Restore the original response body stream
            context.Response.Body = originalBodyStream;
        }

        // Log details
        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context.Request.Method} {context.Request.Path} | User: {userRole} | Status: {context.Response.StatusCode} | Time: {stopwatch.ElapsedMilliseconds}ms";
        _logger.LogInformation(logMessage);
    }
}