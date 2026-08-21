using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler(IRoleService identityRoleService) : ICommandHandler<CreateRoleCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken)
        => await identityRoleService.CreateRoleAsync(command, cancellationToken);
}


