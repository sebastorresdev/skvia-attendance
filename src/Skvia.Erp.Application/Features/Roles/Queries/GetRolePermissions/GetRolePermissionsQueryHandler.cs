using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.DTOs;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Application.Features.Roles.Queries.GetRolePermissions;

public class GetRolePermissionsQueryHandler(IRoleService roleService)
    : IQueryHandler<GetRolePermissionsQuery, ErrorOr<List<PermissionGroupResponse>>>
{
    public Task<ErrorOr<List<PermissionGroupResponse>>> HandleAsync(GetRolePermissionsQuery query, CancellationToken cancellationToken)
        => roleService.GetRolePermissionsAsync(query.RoleId, cancellationToken);
}


