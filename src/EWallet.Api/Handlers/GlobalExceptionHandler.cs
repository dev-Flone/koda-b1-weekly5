using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EWallet.Core.Exceptions;

namespace EWallet.Api.Handlers;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "An unhandled exception occurred."
        );

        var (statusCode, title, detail) = exception switch
        {
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                exception.Message
            ),

            InvalidOperationException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                exception.Message
            ),

            InsufficientBalanceException => (
                StatusCodes.Status409Conflict,
                "Insufficient Balance",
                exception.Message
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred."
            )
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            },
            cancellationToken
        );

        return true;
    }
}