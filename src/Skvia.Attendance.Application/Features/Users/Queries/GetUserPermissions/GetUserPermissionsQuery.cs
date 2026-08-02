using Skvia.Attendance.Application.Common.DTOs;

namespace Skvia.Attendance.Application.Features.Users.Queries.GetUserPermissions;

public record GetUserPermissionsQuery(Guid UserId) : IQuery<ErrorOr<List<PermissionGroupResponse>>>;

