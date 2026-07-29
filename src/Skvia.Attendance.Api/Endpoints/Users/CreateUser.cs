using Skvia.Attendance.Application.Features.Users.Commands.CreateUser;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class CreateUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", Handle)
            .WithSummary("Crear Usuario")
            .RequireAuthorization()
            .Produces<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        CreateUserCommand command,
        ICommandHandler<CreateUserCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            userId => TypedResults.Created($"/api/users/{userId}", userId),
            ResultExtensions.ToProblem);
    }
}
