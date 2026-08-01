using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Users.Commands.CreateUser;

namespace Skvia.Attendance.Api.Endpoints.Users;

public sealed class CreateUser : IEndpoint
{
    public record CreateUserResponse(Guid UserId);
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithName(nameof(CreateUser))
            .WithSummary("Crear usuario")
            .WithDescription("Crea un nuevo usuario en el sistema.")
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        CreateUserCommand command,
        ICommandHandler<CreateUserCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            userId => TypedResults.Created($"/api/v1/users/{userId}", new CreateUserResponse(userId)),
            errors => errors.ToProblem());
    }
}
