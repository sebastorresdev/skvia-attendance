using Microsoft.AspNetCore.Http.HttpResults;

using Skvia.Attendance.Api.Models;

namespace Skvia.Attendance.Api.Common.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var apiProblem = new ProblemResponse
        {
            Status = statusCode,
            Title = string.IsNullOrWhiteSpace(error.Code) ? "ApplicationError" : error.Code,
            Detail = string.IsNullOrWhiteSpace(error.Description) ? "The operation could not be completed." : error.Description,
            Type = GetProblemType(statusCode),
            Errors = null
        };

        return TypedResults.Problem(apiProblem);
    }

    public static IResult ToProblem(List<Error>? errors)
    {
        if (errors is null || errors.Count == 0)
        {
            var internalProblem = new ProblemResponse
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "InternalServerError",
                Detail = "Ocurrió un error inesperado al procesar la solicitud.",
                Type = GetProblemType(StatusCodes.Status500InternalServerError),
                Errors = null
            };
            return TypedResults.Problem(internalProblem);
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ContextualValidationProblem(errors);
        }

        var firstError = errors.First();
        return ToProblem(firstError);
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

    private static ProblemHttpResult ContextualValidationProblem(List<Error> errors)
    {
        var errorsDictionary = errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                group => group.Key,
                group => group.Select(e => e.Description).ToArray()
            );

        var apiProblem = new ProblemResponse
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation.ValidationError",
            Detail = "Errors de validación",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Errors = errorsDictionary
        };

        return TypedResults.Problem(apiProblem);
    }
}
