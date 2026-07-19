using Skvia.Attendance.Api.Common.Extensions;
using Skvia.Attendance.Application.Common.Messaging;
using Skvia.Attendance.Application.Users.Commands.UpdateUser;
using Skvia.Attendance.Contracts.Users;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class UpdateUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{userId:guid}", Handle)
            .WithSummary("Actualizar Usuario")
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        Guid userId,
        UpdateUserRequest request,
        ICommandHandler<UpdateUserCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(userId);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            ResultExtensions.ToProblem);
    }
}

public static class UpdateUserExtension
{
    public static UpdateUserCommand ToCommand(this UpdateUserRequest request, Guid userId)
    {
        return new UpdateUserCommand(
            UserId: userId,
            UserName: request.UserName,
            IsActive: request.IsActive,
            DisplayName: request.DisplayName,
            Email: request.Email,
            PhoneNumber: request.PhoneNumber,
            PhotoUrl: request.PhotoUrl,
            BranchIds: [.. request.BranchIds.Select(id => Guid.TryParse(id, out var guid) ? guid : Guid.Empty)],
            Roles: request.Roles
        );
    }
}
