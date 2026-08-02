namespace Skvia.Attendance.Application.Common.DTOs;

public record PermissionGroupResponse(
    string Group,
    string GroupDescription,
    List<PermissionItemResponse> Permissions
);
