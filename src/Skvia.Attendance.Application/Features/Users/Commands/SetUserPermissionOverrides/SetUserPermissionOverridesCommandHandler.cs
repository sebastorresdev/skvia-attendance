namespace Skvia.Attendance.Application.Features.Users.Commands.SetUserPermissionOverrides;

public class SetUserPermissionOverridesCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<SetUserPermissionOverridesCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(SetUserPermissionOverridesCommand command, CancellationToken cancellationToken)
        => userAccountService.SetPermissionOverridesAsync(command, cancellationToken);
}
