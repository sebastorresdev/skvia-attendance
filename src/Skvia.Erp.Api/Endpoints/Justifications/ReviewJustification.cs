using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Justifications.Commands.ReviewJustification;

namespace Skvia.Erp.Api.Endpoints.Justifications;

public record ReviewJustificationRequest(
    bool Approve,
    string? Notes);

public sealed class ReviewJustification : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{id:guid}/review", Handle)
            .WithName(nameof(ReviewJustification))
            .WithSummary("Aprobar o rechazar una justificación")
            .WithDescription("Permite a un administrador o RRHH aprobar o rechazar una solicitud de justificación.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromBody] ReviewJustificationRequest request,
        ICommandHandler<ReviewJustificationCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new ReviewJustificationCommand(id, request.Approve, request.Notes);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}



