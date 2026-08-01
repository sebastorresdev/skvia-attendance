namespace Skvia.Attendance.Application.Features.Users.DTOs;

public record PermissionItemResponse(
    string Key,
    string Display,
    string Description,
    bool Granted,
    string? Source
);
