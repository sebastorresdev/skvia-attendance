using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Application.Roles.Queries.GetRoles;

public class GetRolesQueryHandler(RoleManager<ApplicationRole> roleManager) : IQueryHandler<GetRolesQuery, ErrorOr<List<RoleResponse>>>
{
    public async Task<ErrorOr<List<RoleResponse>>> HandleAsync(GetRolesQuery query, CancellationToken cancellationToken)
    {
        return await roleManager.Roles
            .Select(r => new RoleResponse(r.Id, r.Name!, r.Description))
            .ToListAsync(cancellationToken);
    }
}
