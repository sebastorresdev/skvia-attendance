using ErrorOr;

using Skvia.Attendance.Api.Common.Extensions;
using Skvia.Attendance.Application.Common.Messaging;
using Skvia.Attendance.Application.Identity.Commands.CreateUser;
using Skvia.Attendance.Contracts.Users;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class CreateUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", Handle)
            .WithSummary("Crear Usuario")
            .Produces(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        CreateUserRequest request,
        ICommandHandler<CreateUserCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            userId => TypedResults.Created($"/api/users/{userId}", new CreateUserResponse(userId)),
            ResultExtensions.ToProblem);
    }
}

public static class CreateUserExtension
{
    public static CreateUserCommand ToCommand(this CreateUserRequest request)
    {
        return new CreateUserCommand(
            UserName: request.UserName,
            Password: request.Password,
            DisplayName: request.DisplayName,
            Email: request.Email,
            PhoneNumber: request.PhoneNumber,
            PhotoUrl: request.PhotoUrl,
            BranchIds: [.. request.BranchIds.Select(id => Guid.TryParse(id, out var guid) ? guid : Guid.Empty)],
            RoleIds: [.. request.RoleIds.Select(id => Guid.TryParse(id, out var guid) ? guid : Guid.Empty)]
        );
    }
}

