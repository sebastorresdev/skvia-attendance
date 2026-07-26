namespace Skvia.Attendance.Application.Branches.DTOs;

public record BranchDetailResponse(
    Guid BranchId,
    string Code,
    string Name,
    string? Address);
