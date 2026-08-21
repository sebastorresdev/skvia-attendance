using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.Workplaces.Commands.DeleteWorkplace;

namespace Skvia.Erp.Api.Endpoints.Workplaces;

public sealed class DeleteWorkplace : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/{id:guid}", Handle)
            .WithName(nameof(DeleteWorkplace))
            .WithSummary("Eliminar un lugar de marcación")
            .WithDescription("Elimina un lugar de marcación existente.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        ICommandHandler<DeleteWorkplaceCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteWorkplaceCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}



