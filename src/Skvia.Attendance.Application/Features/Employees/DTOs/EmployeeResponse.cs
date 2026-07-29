namespace Skvia.Attendance.Application.Features.Employees.DTOs;

public record EmployeeResponse(Guid EmployeeId, string Code, string FirstName, string LastName, string? Department, string? PhotoUrl);
