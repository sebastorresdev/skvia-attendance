using Skvia.Attendance.Application.Common.DTOs;

namespace Skvia.Attendance.Application.Features.Users.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler(IUserAccountService userAccountService)
    : IQueryHandler<GetUserPermissionsQuery, ErrorOr<List<PermissionGroupResponse>>>
{
    public Task<ErrorOr<List<PermissionGroupResponse>>> HandleAsync(GetUserPermissionsQuery query, CancellationToken cancellationToken)
        => userAccountService.GetUserPermissionsAsync(query.UserId, cancellationToken);
}
