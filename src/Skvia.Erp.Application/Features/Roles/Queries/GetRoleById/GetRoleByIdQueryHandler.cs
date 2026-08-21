using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
using Skvia.Erp.Application.Features.Roles.DTOs;

namespace Skvia.Erp.Application.Features.Roles.Queries.GetRoleById;

public class GetRoleByIdQueryHandler(IRoleService identityRoleService) : IQueryHandler<GetRoleByIdQuery, ErrorOr<RoleResponse>>
{
    public async Task<ErrorOr<RoleResponse>> HandleAsync(GetRoleByIdQuery query, CancellationToken cancellationToken)
        => await identityRoleService.GetRoleByIdAsync(query.Id, cancellationToken);
}


