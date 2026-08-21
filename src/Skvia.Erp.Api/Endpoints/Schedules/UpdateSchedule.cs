using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Schedules.Commands.UpdateSchedule;

namespace Skvia.Erp.Api.Endpoints.Schedules;

public sealed class UpdateSchedule : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{id:guid}", Handle)
            .WithName(nameof(UpdateSchedule))
            .WithSummary("Actualizar turno base")
            .WithDescription("Actualiza los datos de un turno predefinido.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        [FromBody] UpdateScheduleRequest request,
        ICommandHandler<UpdateScheduleCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateScheduleCommand(
            id, 
            request.Code, 
            request.Description, 
            request.TimeZoneId, 
            request.DefaultStartTime, 
            request.DefaultEndTime,
            request.HasBreak,
            request.BreakStartTime,
            request.BreakEndTime);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public record UpdateScheduleRequest(
    string Code,
    string Description,
    string TimeZoneId,
    TimeOnly DefaultStartTime, 
    TimeOnly DefaultEndTime,
    bool HasBreak = false,
    TimeOnly? BreakStartTime = null,
    TimeOnly? BreakEndTime = null);



