using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Application.Features.Schedules.Commands.DeleteSchedule;

/// <summary>
/// Comando para eliminar una plantilla de horario.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Schedule.Delete)]
public record DeleteScheduleCommand(Guid Id) : ICommand<ErrorOr<Success>>;



