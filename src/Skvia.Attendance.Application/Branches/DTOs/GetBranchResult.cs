namespace Skvia.Attendance.Application.Branches.DTOs;

public record GetBranchResult(
    Guid Id,
    string Code,
    string Name,
    string? Address);
