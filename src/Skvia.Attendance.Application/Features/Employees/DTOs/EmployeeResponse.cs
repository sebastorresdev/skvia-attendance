using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Features.Employees.DTOs;

public record EmployeeResponse(
    Guid Id,
    string Code,
    string FirstName,
    string LastName,
    DocumentType DocumentType,
    string DocumentNumber,
    string? Email,
    string? Phone,
    string? Department,
    string? Position,
    string? PhotoUrl,
    Guid? MainBranchId,
    string? MainBranchName,
    EmployeeStatus Status);
