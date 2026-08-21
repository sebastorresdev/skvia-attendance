using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Features.Justifications.DTOs;
using Skvia.Erp.Domain.Justifications;
using ErrorOr;

namespace Skvia.Erp.Application.Features.Justifications.Queries.GetJustifications;

/// <summary>
/// Consulta para listar las justificaciones de asistencia.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Justification.View)]
public record GetJustificationsQuery(
    DateOnly? StartDate = null,
    DateOnly? EndDate = null,
    Guid? EmployeeId = null,
    JustificationStatus? Status = null,
    Guid? BranchId = null) : IQuery<ErrorOr<List<JustificationResponse>>>;


