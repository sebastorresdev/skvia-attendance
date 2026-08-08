using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Justifications.Commands.CreateJustification;

namespace Skvia.Attendance.Api.Endpoints.Justifications;

public sealed class CreateJustification : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithName(nameof(CreateJustification))
            .WithSummary("Crear una solicitud de justificación")
            .WithDescription("Registra una solicitud de justificación para tardanza, ausencia o salida temprana.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromBody] CreateJustificationRequest request,
        ICommandHandler<CreateJustificationCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateJustificationCommand(
            request.EmployeeId,
            request.Date,
            request.Type,
            request.Reason,
            request.DocumentUrl);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            id => TypedResults.Created($"/api/v1/justifications/{id}", id),
            errors => errors.ToProblem());
    }
}

public record CreateJustificationRequest(
    Guid EmployeeId,
    DateOnly Date,
    Domain.Justifications.JustificationType Type,
    string Reason,
    string? DocumentUrl);
