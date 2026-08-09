using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Employees;
using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(
    Guid Id,
    string Code,
    string FirstName,
    string LastName,
    DocumentType DocumentType,
    string DocumentNumber,
    DateTimeOffset HireDate,
    string? Email = null,
    string? Phone = null,
    string? Position = null,
    string? Department = null,
    string? PhotoUrl = null,
    Guid? MainBranchId = null,
    bool MobileCheckInEnabled = false,
    string? ApplicationUserId = null,
    bool RequireFourPointAttendance = false,
    bool IsAttendanceTracked = true,
    bool AutoCompleteClockOut = false,
    List<Guid>? AllowedKioskIds = null) : ICommand<ErrorOr<Success>>;
