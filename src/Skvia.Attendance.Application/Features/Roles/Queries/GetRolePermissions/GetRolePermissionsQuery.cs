using Skvia.Attendance.Application.Common.DTOs;

namespace Skvia.Attendance.Application.Features.Roles.Queries.GetRolePermissions;

public record GetRolePermissionsQuery(Guid RoleId) : IQuery<ErrorOr<List<PermissionGroupResponse>>>;
