using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Justifications.DTOs;
using Skvia.Attendance.Application.Features.Justifications.Queries.GetJustifications;
using Skvia.Attendance.Domain.Justifications;

namespace Skvia.Attendance.Api.Endpoints.Justifications;

public sealed class GetJustifications : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithName(nameof(GetJustifications))
            .WithSummary("Obtener solicitudes de justificación")
            .WithDescription("Obtiene la lista de solicitudes de justificación aplicando filtros opcionales.")
            .Produces<List<JustificationResponse>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? employeeId,
        [FromQuery] JustificationStatus? status,
        [FromQuery] Guid? branchId,
        IQueryHandler<GetJustificationsQuery, ErrorOr<List<JustificationResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetJustificationsQuery(startDate, endDate, employeeId, status, branchId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            list => TypedResults.Ok(list),
            errors => errors.ToProblem());
    }
}
