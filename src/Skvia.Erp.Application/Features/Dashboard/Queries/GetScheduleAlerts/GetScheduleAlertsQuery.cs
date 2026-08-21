using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;

namespace Skvia.Erp.Application.Features.Dashboard.Queries.GetScheduleAlerts;

/// <summary>
/// Consulta para obtener alertas del panel de control de horarios y asistencia.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Dashboard.View)]
public record ScheduleAlertDto(Guid EmployeeId, string EmployeeCode, string EmployeeName, DateOnly? LastScheduleDate);

/// <summary>
/// Consulta para obtener alertas del panel de control de horarios y asistencia.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Dashboard.View)]
public record GetScheduleAlertsQuery : IQuery<ErrorOr<List<ScheduleAlertDto>>>;



