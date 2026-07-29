using Skvia.Attendance.Application.Features.Users.DTOs;
using Skvia.Attendance.Application.Features.Users.Queries.GetUsers;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class GetUsers : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithSummary("Obtener usuarios")
            .Produces<List<UserResponse>>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> Handle(
        IQueryHandler<GetUsersQuery, ErrorOr<List<UserResponse>>> handler,
        CancellationToken ct)
    {
        var query = new GetUsersQuery();

        var result = await handler.HandleAsync(query, ct);

        return result.Match(
            TypedResults.Ok,
            ResultExtensions.ToProblem);
    }
}
