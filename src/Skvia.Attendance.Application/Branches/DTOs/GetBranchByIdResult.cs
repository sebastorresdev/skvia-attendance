namespace Skvia.Attendance.Application.Branches.DTOs;

public record GetBranchByIdResult(
    Guid Id,
    string Code,
    string Name,
    string? Address);
