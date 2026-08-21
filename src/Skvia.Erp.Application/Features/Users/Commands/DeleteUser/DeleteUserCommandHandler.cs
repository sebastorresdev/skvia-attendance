using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<DeleteUserCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
        => userAccountService.DeleteUserAsync(command, cancellationToken);
}


