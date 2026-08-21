using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Application.Features.Schedules.Commands.UpdateSchedule;

/// <summary>
/// Comando para actualizar una plantilla de horario existente.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Schedule.Update)]
public record UpdateScheduleCommand(
    Guid Id,
    string Code,
    string Description,
    string TimeZoneId,
    TimeOnly DefaultStartTime,
    TimeOnly DefaultEndTime,
    bool HasBreak = false,
    TimeOnly? BreakStartTime = null,
    TimeOnly? BreakEndTime = null) : ICommand<ErrorOr<Success>>;



