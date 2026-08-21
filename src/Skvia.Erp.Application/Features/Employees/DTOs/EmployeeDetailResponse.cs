using Skvia.Erp.Domain.Employees;

namespace Skvia.Erp.Application.Features.Employees.DTOs;

public record EmployeeDetailResponse(
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
    DateTimeOffset HireDate,
    string? PhotoUrl,
    Guid? MainBranchId,
    string? MainBranchName,
    EmployeeStatus Status,
    bool MobileCheckInEnabled,
    string? ApplicationUserId,
    bool RequireFourPointAttendance,
    bool IsAttendanceTracked,
    bool AutoCompleteClockOut,
    int TardinessToleranceMinutes,
    List<Guid> AllowedWorkplaceIds);

