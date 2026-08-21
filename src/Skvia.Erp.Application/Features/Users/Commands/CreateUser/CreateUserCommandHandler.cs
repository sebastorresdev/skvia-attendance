using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<CreateUserCommand, ErrorOr<Guid>>
{
    public Task<ErrorOr<Guid>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
        => userAccountService.CreateUserAsync(command, cancellationToken);
}


