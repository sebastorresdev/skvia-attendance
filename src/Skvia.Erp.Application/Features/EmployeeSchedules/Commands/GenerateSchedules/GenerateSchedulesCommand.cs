using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Common;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.Commands.GenerateSchedules;

/// <summary>
/// Comando para generar la grilla de horarios de empleados.
/// </summary>
[AuthorizeCommand(Permissions = Permission.EmployeeSchedule.Generate)]
public record GenerateSchedulesCommand(
    Guid EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate,
    List<SchedulePatternInput>? Patterns = null) : ICommand<ErrorOr<Success>>;

/// <summary>
/// Comando para generar la grilla de horarios de empleados.
/// </summary>
[AuthorizeCommand(Permissions = Permission.EmployeeSchedule.Generate)]
public record SchedulePatternInput(
    DayOfWeek DayOfWeek,
    bool IsWorkDay,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    Guid? BaseScheduleId = null);



