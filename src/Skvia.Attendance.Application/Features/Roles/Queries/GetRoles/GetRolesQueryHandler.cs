using Skvia.Attendance.Application.Features.Roles.DTOs;

namespace Skvia.Attendance.Application.Features.Roles.Queries.GetRoles;

public class GetRolesQueryHandler(IIdentityRoleService identityRoleService) : IQueryHandler<GetRolesQuery, ErrorOr<List<RoleResponse>>>
{
    public async Task<ErrorOr<List<RoleResponse>>> HandleAsync(GetRolesQuery query, CancellationToken cancellationToken)
        => await identityRoleService.GetRolesAsync(cancellationToken);
}
