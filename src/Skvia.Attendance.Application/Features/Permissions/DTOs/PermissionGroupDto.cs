namespace Skvia.Attendance.Application.Features.Permissions.DTOs;

public record PermissionGroupDto(string Group, string GroupDescription, List<PermissionItemDto> Permissions);
