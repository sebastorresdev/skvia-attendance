using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Common.Models;
using Skvia.Attendance.Application.Features.Auth.Queries.GetCurrentUser;

namespace Skvia.Attendance.Api.Endpoints.Auth;

public class GetCurrentUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/me", handle)
            .WithSummary("Obtiene información del usuario autenticado.")
            .RequireAuthorization()
            .Produces<CurrentUser>()
            .Produces<ProblemResponse>(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> handle(
        IQueryHandler<GetCurrentUserQuery, ErrorOr<CurrentUser>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetCurrentUserQuery();

        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            ResultExtensions.ToProblem);
    }
}
