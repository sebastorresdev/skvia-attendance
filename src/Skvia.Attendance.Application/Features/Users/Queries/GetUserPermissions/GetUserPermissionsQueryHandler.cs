using Skvia.Attendance.Application.Features.Users.DTOs;

namespace Skvia.Attendance.Application.Features.Users.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler(IUserAccountService userAccountService)
    : IQueryHandler<GetUserPermissionsQuery, ErrorOr<List<PermissionGroupDto>>>
{
    public Task<ErrorOr<List<PermissionGroupDto>>> HandleAsync(GetUserPermissionsQuery query, CancellationToken cancellationToken)
        => userAccountService.GetUserPermissionsAsync(query.UserId, cancellationToken);
}
