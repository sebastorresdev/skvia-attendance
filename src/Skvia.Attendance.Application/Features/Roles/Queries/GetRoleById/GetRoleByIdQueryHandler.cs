using Skvia.Attendance.Application.Features.Roles.DTOs;

namespace Skvia.Attendance.Application.Features.Roles.Queries.GetRoleById;

public class GetRoleByIdQueryHandler(IRoleService identityRoleService) : IQueryHandler<GetRoleByIdQuery, ErrorOr<RoleResponse>>
{
    public async Task<ErrorOr<RoleResponse>> HandleAsync(GetRoleByIdQuery query, CancellationToken cancellationToken)
        => await identityRoleService.GetRoleByIdAsync(query.Id, cancellationToken);
}
