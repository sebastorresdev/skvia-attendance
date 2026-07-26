namespace Skvia.Attendance.Application.Branches.DTOs;

public record BranchResponse(
    Guid BranchId,
    string Code,
    string Name,
    string? Address);
