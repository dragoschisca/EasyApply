using System.Net;
using System.Text.Json;
using EasyApply.Domain.Exceptions;

namespace EasyApply.Api.Middleware;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case NotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            case BusinessException:
                code = HttpStatusCode.BadRequest;
                break;
            case ConflictException:
                code = HttpStatusCode.Conflict;
                break;
            case ForbiddenException:
                code = HttpStatusCode.Forbidden;
                break;
            case UnauthorizedException:
                code = HttpStatusCode.Unauthorized;
                break;
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { message = validationException.Message, errors = validationException.Errors });
                break;
        }

        if (string.IsNullOrEmpty(result))
        {
            result = JsonSerializer.Serialize(new { message = exception.Message });
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
