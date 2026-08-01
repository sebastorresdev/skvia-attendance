namespace Skvia.Attendance.Application.Features.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<ResetPasswordCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
        => userAccountService.ResetPasswordAsync(command, cancellationToken);
}
