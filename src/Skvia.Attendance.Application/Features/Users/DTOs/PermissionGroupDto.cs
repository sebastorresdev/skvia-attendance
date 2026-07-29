namespace Skvia.Attendance.Application.Features.Users.DTOs;

public record PermissionGroupDto(
    string Group,
    string GroupDescription,
    List<PermissionItemDto> Permissions
);
