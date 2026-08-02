namespace Skvia.Attendance.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommandHandler(IIdentityRoleService identityRoleService) : ICommandHandler<DeleteRoleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteRoleCommand command, CancellationToken cancellationToken)
        => await identityRoleService.DeleteRoleAsync(command, cancellationToken);
}
