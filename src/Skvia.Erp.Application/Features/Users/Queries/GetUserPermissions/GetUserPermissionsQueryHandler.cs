using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
using Skvia.Erp.Application.Common.DTOs;

namespace Skvia.Erp.Application.Features.Users.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler(IUserAccountService userAccountService)
    : IQueryHandler<GetUserPermissionsQuery, ErrorOr<List<PermissionGroupResponse>>>
{
    public Task<ErrorOr<List<PermissionGroupResponse>>> HandleAsync(GetUserPermissionsQuery query, CancellationToken cancellationToken)
        => userAccountService.GetUserPermissionsAsync(query.UserId, cancellationToken);
}


