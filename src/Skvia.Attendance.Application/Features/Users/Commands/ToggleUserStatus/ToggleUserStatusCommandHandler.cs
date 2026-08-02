using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.Users.Commands.ToggleUserStatus;

public class ToggleUserStatusCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<ToggleUserStatusCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(ToggleUserStatusCommand command, CancellationToken cancellationToken)
        => userAccountService.ToggleUserStatusAsync(command, cancellationToken);
}
