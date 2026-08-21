using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Domain.Common;
using Skvia.Erp.Domain.Employees;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Security;

namespace Skvia.Erp.Application.Features.Employees.Commands.CreateEmployee;

/// <summary>
/// Comando para crear un nuevo empleado en el sistema.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Employee.Create)]
public record CreateEmployeeCommand(
    string Code,
    string FirstName,
    string LastName,
    DocumentType DocumentType,
    string DocumentNumber,
    DateTimeOffset HireDate,
    string? Email = null,
    string? Phone = null,
    string? Position = null,
    Guid? DepartmentId = null,
    string? PhotoUrl = null,
    Guid? MainBranchId = null,
    bool MobileCheckInEnabled = false,
    string? ApplicationUserId = null,
    bool RequireFourPointAttendance = false,
    bool IsAttendanceTracked = true,
    bool AutoCompleteClockOut = false,
    int TardinessToleranceMinutes = 0,
    List<Guid>? AllowedWorkplaceIds = null) : ICommand<ErrorOr<Guid>>;

