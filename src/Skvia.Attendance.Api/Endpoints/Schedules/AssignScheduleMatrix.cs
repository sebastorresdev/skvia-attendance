using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.AssignScheduleMatrix;

namespace Skvia.Attendance.Api.Endpoints.Schedules;

public sealed class AssignScheduleMatrix : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/assign-matrix", Handle)
            .WithName(nameof(AssignScheduleMatrix))
            .WithSummary("Guardar matriz de programación rotativa")
            .WithDescription("Guarda la asignación de turnos dinámicos o rotativos para múltiples empleados y días.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromBody] AssignScheduleMatrixRequest request,
        ICommandHandler<AssignScheduleMatrixCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new AssignScheduleMatrixCommand(request.Cells);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.Ok(),
            errors => errors.ToProblem());
    }
}

public record AssignScheduleMatrixRequest(
    List<ScheduleMatrixCellItem> Cells);
