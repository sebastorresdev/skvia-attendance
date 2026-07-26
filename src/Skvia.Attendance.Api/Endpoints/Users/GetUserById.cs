using Skvia.Attendance.Application.Users.DTOs;
using Skvia.Attendance.Application.Users.Queries.GetUserById;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class GetUserById : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{userId:guid}", Handle)
            .WithSummary("Obtener usuario por ID")
            .Produces<UserDetailResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        Guid userId,
        IQueryHandler<GetUserByIdQuery, ErrorOr<UserDetailResponse>> handler,
        CancellationToken ct)
    {
        var query = new GetUserByIdQuery(userId);

        var result = await handler.HandleAsync(query, ct);

        return result.Match(
            TypedResults.Ok,
            ResultExtensions.ToProblem);
    }
}
