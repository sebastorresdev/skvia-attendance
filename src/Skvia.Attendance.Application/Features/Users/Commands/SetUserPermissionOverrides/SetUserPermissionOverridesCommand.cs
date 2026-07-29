namespace Skvia.Attendance.Application.Features.Users.Commands.SetUserPermissionOverrides;

public record SetUserPermissionOverridesCommand(
    Guid UserId,
    List<string> PermissionKeys
) : ICommand<ErrorOr<Success>>;
