using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.Schedules.DTOs;

namespace Skvia.Erp.Application.Features.Schedules.Queries.GetSchedules;

/// <summary>
/// Consulta para obtener las plantillas de horario disponibles.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Schedule.View)]
public record GetSchedulesQuery : IQuery<ErrorOr<List<ScheduleResponse>>>;



