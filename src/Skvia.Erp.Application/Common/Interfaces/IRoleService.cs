using ErrorOr;
using Skvia.Erp.Application.Common.DTOs;
using Skvia.Erp.Application.Features.Roles.Commands.CreateRole;
using Skvia.Erp.Application.Features.Roles.Commands.DeleteRole;
using Skvia.Erp.Application.Features.Roles.Commands.UpdateRole;
using Skvia.Erp.Application.Features.Roles.Commands.SetRolePermissions;
using Skvia.Erp.Application.Features.Roles.DTOs;

namespace Skvia.Erp.Application.Common.Interfaces;

public interface IRoleService
{
    Task<ErrorOr<Guid>> CreateRoleAsync(CreateRoleCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> UpdateRoleAsync(UpdateRoleCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> DeleteRoleAsync(DeleteRoleCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<RoleResponse>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken);
    Task<ErrorOr<List<RoleResponse>>> GetRolesAsync(CancellationToken cancellationToken);
    Task<ErrorOr<List<PermissionGroupResponse>>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> SetRolePermissionsAsync(SetRolePermissionsCommand command, CancellationToken cancellationToken);
}


