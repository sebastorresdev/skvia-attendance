namespace Skvia.Attendance.Api.Endpoints.Branches.Requests;

public record UpdateBranchRequest(string Code, string Name, string? Address);
