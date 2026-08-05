using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Features.Employees.DTOs;

public record EmployeeDetailResponse(
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
    DateTimeOffset HireDate,
    string? PhotoUrl,
    Guid? MainBranchId,
    string? MainBranchName,
    EmployeeStatus Status,
    bool MobileCheckInEnabled,
    string? ApplicationUserId);
