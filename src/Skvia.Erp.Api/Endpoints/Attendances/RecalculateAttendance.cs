using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Attendances.Commands.RecalculateAttendance;

namespace Skvia.Erp.Api.Endpoints.Attendances;

public sealed class RecalculateAttendance : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{id:guid}/recalculate", Handle)
            .WithName(nameof(RecalculateAttendance))
            .WithSummary("Recalcular asistencia")
            .WithDescription("Recalcula la tardanza y horas extra de un registro de asistencia en base al horario actual.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<RecalculateAttendanceCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new RecalculateAttendanceCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}



