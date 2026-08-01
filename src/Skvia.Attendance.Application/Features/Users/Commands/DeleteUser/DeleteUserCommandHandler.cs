namespace Skvia.Attendance.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<DeleteUserCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
        => userAccountService.DeleteUserAsync(command, cancellationToken);
}
