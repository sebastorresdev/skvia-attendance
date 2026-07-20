namespace Skvia.Attendance.Contracts.Branch;

public record BranchDetailResponse(
    Guid Id,
    string Code,
    string Name,
    string? Address);
