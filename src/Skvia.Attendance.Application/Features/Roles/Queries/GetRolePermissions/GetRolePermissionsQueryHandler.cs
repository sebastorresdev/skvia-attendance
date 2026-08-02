using Skvia.Attendance.Application.Common.DTOs;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.Roles.Queries.GetRolePermissions;

public class GetRolePermissionsQueryHandler(IRoleService roleService)
    : IQueryHandler<GetRolePermissionsQuery, ErrorOr<List<PermissionGroupResponse>>>
{
    public Task<ErrorOr<List<PermissionGroupResponse>>> HandleAsync(GetRolePermissionsQuery query, CancellationToken cancellationToken)
        => roleService.GetRolePermissionsAsync(query.RoleId, cancellationToken);
}
