using Skvia.Attendance.Application.Features.Permissions.DTOs;

namespace Skvia.Attendance.Application.Features.Permissions.Queries.GetPermissions;

public record GetPermissionsQuery : IQuery<ErrorOr<List<PermissionGroupDto>>>;
