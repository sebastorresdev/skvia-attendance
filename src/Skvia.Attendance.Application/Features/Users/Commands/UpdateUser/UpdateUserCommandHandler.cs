namespace Skvia.Attendance.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<UpdateUserCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
        => userAccountService.UpdateUserAsync(command, cancellationToken);
}
