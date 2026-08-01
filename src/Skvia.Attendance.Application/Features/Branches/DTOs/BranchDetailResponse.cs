namespace Skvia.Attendance.Application.Features.Branches.DTOs;

public record BranchDetailResponse(
    Guid Id,
    string Code,
    string Name,
    string? Address);
