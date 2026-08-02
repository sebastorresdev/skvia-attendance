namespace Skvia.Attendance.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandler(IRoleService identityRoleService) : ICommandHandler<UpdateRoleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(UpdateRoleCommand command, CancellationToken cancellationToken)
        => await identityRoleService.UpdateRoleAsync(command, cancellationToken);
}
