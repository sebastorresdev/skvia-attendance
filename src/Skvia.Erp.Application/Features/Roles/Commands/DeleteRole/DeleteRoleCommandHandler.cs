using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommandHandler(IRoleService identityRoleService) : ICommandHandler<DeleteRoleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteRoleCommand command, CancellationToken cancellationToken)
        => await identityRoleService.DeleteRoleAsync(command, cancellationToken);
}


