using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Features.EmployeeSchedules.DTOs;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.Queries.GetResolvedEmployeeSchedule;

/// <summary>
/// Consulta para obtener el horario resuelto de un empleado.
/// </summary>
[AuthorizeCommand(Permissions = Permission.EmployeeSchedule.View)]
public record GetResolvedEmployeeScheduleQuery(
    Guid EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate) : IQuery<ErrorOr<List<ResolvedScheduleDayDto>>>;



