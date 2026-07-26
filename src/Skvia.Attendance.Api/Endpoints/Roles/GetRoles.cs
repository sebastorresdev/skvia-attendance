using Skvia.Attendance.Application.Roles.Queries.GetRoles;

namespace Skvia.Attendance.Api.Endpoints.Roles;

public class GetRoles : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithSummary("Obtener roles")
            .Produces<List<RoleResponse>>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> Handle(
        IQueryHandler<GetRolesQuery, ErrorOr<List<RoleResponse>>> handler,
        CancellationToken ct)
    {
        var query = new GetRolesQuery();

        var result = await handler.HandleAsync(query, ct);

        return result.Match(
            TypedResults.Ok,
            ResultExtensions.ToProblem);
    }
}
