namespace Skvia.Attendance.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<CreateUserCommand, ErrorOr<Guid>>
{
    public Task<ErrorOr<Guid>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
        => userAccountService.CreateUserAsync(command, cancellationToken);
}
