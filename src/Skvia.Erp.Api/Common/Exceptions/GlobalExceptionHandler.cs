using ErrorOr;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Npgsql;

namespace Skvia.Erp.Api.Common.Exceptions;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unhandled exception {ExceptionType}. TraceId: {TraceId}. Path: {Path}. Method: {Method}",
            exception.GetType().Name,
            httpContext.TraceIdentifier,
            httpContext.Request.Path,
            httpContext.Request.Method);

        var (statusCode, title, detail) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = GetProblemType(statusCode),
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",

            // Si MapException devuelve un detalle lo usamos.
            // Si no, GetSafeErrorMessage decide qué mostrar dependiendo del entorno.
            Detail = detail ?? GetSafeErrorMessage(exception, httpContext),

            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
                ["timestamp"] = DateTime.UtcNow
            }
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static (int StatusCode, string Title, string? Detail) MapException(Exception exception)
    {
        // PostgreSQL
        if (TryFindPostgresException(exception, out var pgEx))
        {
            return pgEx.SqlState switch
            {
                "23505" => (
                    StatusCodes.Status409Conflict,
                    "Database Conflict",
                    "The record already exists."
                ),

                "23503" => (
                    StatusCodes.Status409Conflict,
                    "Database Link Error",
                    "The operation is not allowed because this record is linked to other data."
                ),

                "23502" => (
                    StatusCodes.Status400BadRequest,
                    "Database Constraint Error",
                    "Required data fields are missing."
                ),

                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Database Error",
                    null
                )
            };
        }

        return exception switch
        {
            ArgumentNullException => (
                StatusCodes.Status400BadRequest,
                "Required Parameter",
                null
            ),

            ArgumentOutOfRangeException => (
                StatusCodes.Status400BadRequest,
                "Parameter Out Of Range",
                null
            ),

            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Invalid Parameter",
                null
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Security Error",
                "Access to this resource is denied."
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                null
            )
        };
    }

    private static bool TryFindPostgresException(Exception? ex, out PostgresException postgresException)
    {
        while (ex is not null)
        {
            if (ex is PostgresException pgEx)
            {
                postgresException = pgEx;
                return true;
            }

            ex = ex.InnerException;
        }

        postgresException = null!;
        return false;
    }

    private static string GetSafeErrorMessage(Exception exception, HttpContext context)
    {
        var env = context.RequestServices.GetRequiredService<IHostEnvironment>();

        if (env.IsDevelopment())
        {
            // En desarrollo mostramos el mensaje real para facilitar el debugging.
            return exception.Message;
        }

        // En producción ocultamos detalles internos.
        return "An unexpected error occurred. Please contact system support if the issue persists.";
    }

    private static string GetProblemType(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
    };
}


