namespace Skvia.Attendance.Application.Features.Users.DTOs;

public record PermissionGroupResponse(
    string Group,
    string GroupDescription,
    List<PermissionItemResponse> Permissions
);
