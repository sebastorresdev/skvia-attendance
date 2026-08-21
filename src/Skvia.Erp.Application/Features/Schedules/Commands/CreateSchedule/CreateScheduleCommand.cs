using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Application.Features.Schedules.Commands.CreateSchedule;

/// <summary>
/// Comando para crear una plantilla de horario/turno.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Schedule.Create)]
public record CreateScheduleCommand(
    string Code,
    string Description,
    string TimeZoneId,
    TimeOnly DefaultStartTime,
    TimeOnly DefaultEndTime,
    bool HasBreak = false,
    TimeOnly? BreakStartTime = null,
    TimeOnly? BreakEndTime = null) : ICommand<ErrorOr<Guid>>;



