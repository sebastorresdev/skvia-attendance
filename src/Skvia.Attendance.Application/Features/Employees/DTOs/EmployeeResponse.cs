using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Features.Employees.DTOs;

public record EmployeeResponse(
    Guid EmployeeId,
    string Code,
    string FirstName,
    string LastName,
    DocumentType DocumentType,
    string DocumentNumber,
    string? Email,
    string? Phone,
    string? Department,
    string? PhotoUrl);
