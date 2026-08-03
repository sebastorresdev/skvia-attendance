using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Employees;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Employees.Commands.ChangeEmployeeStatus;

public record ChangeEmployeeStatusCommand(
    Guid EmployeeId,
    EmployeeStatus NewStatus) : ICommand<ErrorOr<Success>>;
