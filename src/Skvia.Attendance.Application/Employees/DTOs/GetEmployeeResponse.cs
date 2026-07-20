namespace Skvia.Attendance.Application.Employees.DTOs;

public record GetEmployeeResponse(Guid Id, string Code, string FirstName, string LastName, string? Department, string? PhotoUrl);
