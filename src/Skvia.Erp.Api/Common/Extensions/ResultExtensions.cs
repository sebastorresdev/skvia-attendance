using Skvia.Erp.Api.Common.Extensions;
using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;

using Skvia.Erp.Api.Models;

namespace Skvia.Erp.Api.Common.Extensions;

// Extensions/ResultExtensions.cs
public static class ResultExtensions
{
    public static IResult ToProblem(this Error error)
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

        return TypedResults.Problem(new ApiProblemDetails
        {
            Status = statusCode,
            Title = string.IsNullOrWhiteSpace(error.Code)
                ? "ApplicationError"
                : error.Code,
            Detail = string.IsNullOrWhiteSpace(error.Description)
                ? "The operation could not be completed."
                : error.Description,
            Type = GetProblemType(statusCode),
            Errors = null
        });
    }

    public static IResult ToProblem(this List<Error>? errors)
    {
        if (errors is null || errors.Count == 0)
            return TypedResults.Problem(new ApiProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "InternalServerError",
                Detail = "Ocurrió un error inesperado al procesar la solicitud.",
                Type = GetProblemType(StatusCodes.Status500InternalServerError)
            });

        if (errors.All(e => e.Type == ErrorType.Validation))
            return ToValidationProblem(errors);

        return errors.First().ToProblem();
    }

    public static IResult ToProblem(this IEnumerable<Error> errors)
        => errors.ToList().ToProblem();

    private static ProblemHttpResult ToValidationProblem(List<Error> errors)
    {
        var errorsDictionary = errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description).ToArray());

        return TypedResults.Problem(new ApiProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation.ValidationError",
            Detail = "Se encontraron errores de validación.",
            Type = GetProblemType(StatusCodes.Status400BadRequest),
            Errors = errorsDictionary
        });
    }

    private static string GetProblemType(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        500 => "https://tools.ietf.org/html/rfc9110#section-15.6.1",
        _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
    };
}



