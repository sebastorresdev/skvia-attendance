using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Employees.DTOs;

public record EmployeeDetailResponse(
    Guid EmployeeId,
    string Code,
    string FirstName,
    string LastName,
    DocumentType DocumentType,
    string DocumentNumber,
    string? Email,
    string? Phone,
    string? Position,
    string? Department,
    DateTimeOffset HireDate,
    string? PhotoUrl);
