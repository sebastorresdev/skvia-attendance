namespace Skvia.Attendance.Api.Endpoints.Branches.Requests;

public record CreateBranchRequest(string Code, string Name, string? Address, int TardinessToleranceMinutes = 0);
