using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Schedules.Commands.CreateSchedule;

namespace Skvia.Erp.Api.Endpoints.Schedules;

public sealed class CreateSchedule : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithName(nameof(CreateSchedule))
            .WithSummary("Crear turno base")
            .WithDescription("Crea un nuevo turno predefinido.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromBody] CreateScheduleRequest request,
        ICommandHandler<CreateScheduleCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateScheduleCommand(
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
            id => TypedResults.CreatedAtRoute(id, nameof(GetSchedules), new { id }),
            errors => errors.ToProblem());
    }
}

public record CreateScheduleRequest(
    string Code,
    string Description,
    string TimeZoneId,
    TimeOnly DefaultStartTime, 
    TimeOnly DefaultEndTime,
    bool HasBreak = false,
    TimeOnly? BreakStartTime = null,
    TimeOnly? BreakEndTime = null);



