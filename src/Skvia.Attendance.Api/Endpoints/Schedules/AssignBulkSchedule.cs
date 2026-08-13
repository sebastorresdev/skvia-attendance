using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.AssignBulkSchedule;

namespace Skvia.Attendance.Api.Endpoints.Schedules;

public sealed class AssignBulkSchedule : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/assign-bulk", Handle)
            .WithName(nameof(AssignBulkSchedule))
            .WithSummary("Asignar horario base en lote")
            .WithDescription("Asigna una plantilla de horario base a múltiples empleados para un rango de vigencia.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromBody] AssignBulkScheduleRequest request,
        ICommandHandler<AssignBulkScheduleCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new AssignBulkScheduleCommand(
            request.ScheduleTemplateId,
            request.EmployeeIds,
            request.EffectiveFrom,
            request.EffectiveTo);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.Ok(),
            errors => errors.ToProblem());
    }
}

public record AssignBulkScheduleRequest(
    Guid ScheduleTemplateId,
    List<Guid> EmployeeIds,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo);
