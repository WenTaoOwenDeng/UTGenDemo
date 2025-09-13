using System.Net;
using System.Text.Json;

namespace UTGenDemo.Controller.Middleware;

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
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            ArgumentNullException argumentNullEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = argumentNullEx.Message,
                Details = "Required parameter is null or empty"
            },
            ArgumentException argumentEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = argumentEx.Message,
                Details = "Invalid argument provided"
            },
            KeyNotFoundException keyNotFoundEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = keyNotFoundEx.Message,
                Details = "The requested resource was not found"
            },
            UnauthorizedAccessException unauthorizedEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Message = unauthorizedEx.Message,
                Details = "Access denied"
            },
            InvalidOperationException invalidOpEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = invalidOpEx.Message,
                Details = "Invalid operation"
            },
            _ => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An internal server error occurred",
                Details = "Please try again later or contact support"
            }
        };

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}