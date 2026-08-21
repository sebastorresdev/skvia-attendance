using Skvia.Erp.Domain.Employees;

namespace Skvia.Erp.Application.Features.Employees.DTOs;

public record EmployeeResponse(
    Guid Id,
    string Code,
    string FirstName,
    string LastName,
    DocumentType DocumentType,
    string DocumentNumber,
    string? Email,
    string? Phone,
    Guid? DepartmentId,
    string? Position,
    string? PhotoUrl,
    Guid? MainBranchId,
    string? MainBranchName,
    EmployeeStatus Status,
    bool MobileCheckInEnabled,
    string? ApplicationUserId);

