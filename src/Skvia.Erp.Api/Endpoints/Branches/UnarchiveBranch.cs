using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Branches.Commands.UnarchiveBranch;

namespace Skvia.Erp.Api.Endpoints.Branches;

public sealed class UnarchiveBranch : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}/unarchive", Handle)
            .WithName(nameof(UnarchiveBranch))
            .WithSummary("Desarchivar sucursal")
            .WithDescription("Desarchiva una sucursal en el sistema.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<UnarchiveBranchCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UnarchiveBranchCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}



