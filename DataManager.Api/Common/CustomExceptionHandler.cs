using System.Net;
using System.Text.Json;
using DataManager.Api.Validation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DataManager.Api.Common;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is RequestValidationException requestValidationException)
        {
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request",
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Bad Request",
                Detail = JsonSerializer.Serialize(requestValidationException.Errors)
            }, cancellationToken: cancellationToken);
        }
        
        var exceptionMessage = exception.Message;
        _logger.LogError("Error Message: {ExceptionMessage}", exceptionMessage);

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