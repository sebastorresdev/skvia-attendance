using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Domain.EmployeeSchedules;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.Commands.AssignWeeklySchedule;

/// <summary>
/// Comando para asignar un horario semanal a un empleado.
/// </summary>
[AuthorizeCommand(Permissions = Permission.EmployeeSchedule.Assign)]
public record DailyScheduleRequest(
    DateOnly Date,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    ScheduleDayType DayType,
    Guid? BaseScheduleId = null);

/// <summary>
/// Comando para asignar un horario semanal a un empleado.
/// </summary>
[AuthorizeCommand(Permissions = Permission.EmployeeSchedule.Assign)]
public record AssignWeeklyScheduleCommand(
    Guid EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate,
    List<DailyScheduleRequest> Days) : ICommand<ErrorOr<Success>>;



