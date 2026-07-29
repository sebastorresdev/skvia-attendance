namespace Skvia.Attendance.Application.Features.Users.DTOs;

public record PermissionItemDto(
    string Key,
    string Display,
    string Description,
    bool Granted,
    string? Source
);
