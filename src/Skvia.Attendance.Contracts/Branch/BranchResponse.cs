namespace Skvia.Attendance.Contracts.Branch;

public record BranchResponse(
    Guid Id,
    string Code,
    string Name,
    string? Address);
