using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Auth.DTOs;
using Skvia.Attendance.Application.Features.Auth.Queries.GetCurrentUser;

namespace Skvia.Attendance.Api.Endpoints.Auth;

public sealed class GetCurrentUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/me", Handle)
            .WithName(nameof(GetCurrentUser))
            .WithSummary("Obtener usuario autenticado")
            .WithDescription("Retorna la información del usuario que se encuentra actualmente autenticado en la sesión.")
            .Produces<CurrentUserResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        IQueryHandler<GetCurrentUserQuery, ErrorOr<CurrentUserResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetCurrentUserQuery();
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}
