namespace Skvia.Attendance.Application.Features.Roles.DTOs;

public record RoleResponse(Guid Id, string Name, string? Description, DateTime LastModifiedAt);
