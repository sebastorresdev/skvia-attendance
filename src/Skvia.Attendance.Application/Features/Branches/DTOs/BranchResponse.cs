namespace Skvia.Attendance.Application.Features.Branches.DTOs;

public record BranchResponse(
    Guid Id,
    string Code,
    string Name,
    string? Address);
