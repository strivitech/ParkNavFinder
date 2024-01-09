using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebSocketManager.Infrastructure;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var exceptionMessage = exception.Message;
        _logger.LogError(
            "Error Message: {ExceptionMessage}, Time of occurrence {Time}",
            exceptionMessage, DateTime.UtcNow);

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An error occurred while processing your request.",
            Detail = "An unexpected error occurred. Please try again later."
        }, cancellationToken: cancellationToken);

        return true;
    }
}