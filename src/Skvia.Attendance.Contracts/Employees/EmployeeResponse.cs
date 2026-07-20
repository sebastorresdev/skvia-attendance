namespace Skvia.Attendance.Contracts.Employees;

public record EmployeeResponse(Guid Id, string Code, string FirstName, string LastName, string? Department, string? PhotoUrl);
