namespace Skvia.Attendance.Application.Features.Branches.DTOs;

public record BranchDetailResponse(
    Guid BranchId,
    string Code,
    string Name,
    string? Address);
