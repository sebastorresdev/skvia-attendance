using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Domain.EmployeeSchedules;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.Commands.CreateScheduleException;

/// <summary>
/// Comando para crear una excepción de horario para un empleado.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Schedule.Update)]
public record CreateScheduleExceptionCommand(
    Guid EmployeeId,
    DateOnly Date,
    ScheduleDayType DayType,
    Guid? CustomScheduleId,
    bool IsDayOff,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Reason) : ICommand<ErrorOr<Guid>>;



