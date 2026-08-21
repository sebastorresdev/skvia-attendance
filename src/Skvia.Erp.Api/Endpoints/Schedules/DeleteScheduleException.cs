using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.EmployeeSchedules.Commands.DeleteScheduleException;

namespace Skvia.Erp.Api.Endpoints.Schedules;

public sealed class DeleteScheduleException : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/exceptions/{id:guid}", Handle)
            .WithName(nameof(DeleteScheduleException))
            .WithSummary("Eliminar excepción de horario")
            .WithDescription("Elimina una excepción registrada restaurando el horario base por defecto.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<DeleteScheduleExceptionCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteScheduleExceptionCommand(id);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}



