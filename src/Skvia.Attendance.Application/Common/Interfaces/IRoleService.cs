using Skvia.Attendance.Application.Common.DTOs;
using Skvia.Attendance.Application.Features.Roles.Commands.CreateRole;
using Skvia.Attendance.Application.Features.Roles.Commands.DeleteRole;
using Skvia.Attendance.Application.Features.Roles.Commands.UpdateRole;
using Skvia.Attendance.Application.Features.Roles.DTOs;

namespace Skvia.Attendance.Application.Common.Interfaces;

public interface IRoleService
{
    Task<ErrorOr<Guid>> CreateRoleAsync(CreateRoleCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> UpdateRoleAsync(UpdateRoleCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> DeleteRoleAsync(DeleteRoleCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<RoleResponse>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken);
    Task<ErrorOr<List<RoleResponse>>> GetRolesAsync(CancellationToken cancellationToken);
    Task<ErrorOr<List<PermissionGroupResponse>>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken);
}
