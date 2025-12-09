using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace arabia.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        var response = context.Response;

        response.ContentType = "application/json";

        ErrorResponse errorResponse;

        switch (exception)
        {
            case AppException appException:
                response.StatusCode = appException.StatusCode;
                errorResponse = new ErrorResponse
                {
                    Code = appException.Code,
                    Message = appException.Message,
                    Status = appException.StatusCode,
                    TraceId = traceId,
                };
                break;

            case DbUpdateException dbException:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = new ErrorResponse
                {
                    Code = "database_error",
                    Message = "A database error occurred. Please try again later.",
                    Status = (int)HttpStatusCode.InternalServerError,
                    TraceId = traceId,
                };
                break;

            case InvalidOperationException invalidOpException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse
                {
                    Code = "invalid_operation",
                    Message = invalidOpException.Message,
                    Status = (int)HttpStatusCode.BadRequest,
                    TraceId = traceId,
                };
                break;

            case KeyNotFoundException keyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse = new ErrorResponse
                {
                    Code = "resource_not_found",
                    Message = keyNotFoundException.Message,
                    Status = (int)HttpStatusCode.NotFound,
                    TraceId = traceId,
                };
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = new ErrorResponse
                {
                    Code = "internal_server_error",
                    Message = "An unexpected error occurred. Please try again later.",
                    Status = (int)HttpStatusCode.InternalServerError,
                    TraceId = traceId,
                };
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(
            errorResponse,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        return response.WriteAsync(jsonResponse);
    }
}
