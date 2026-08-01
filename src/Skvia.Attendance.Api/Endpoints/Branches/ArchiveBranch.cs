using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Branches.Commands.ArchiveBranch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public sealed class ArchiveBranch : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}/archive", Handle)
            .WithName(nameof(ArchiveBranch))
            .WithSummary("Archivar sucursal")
            .WithDescription("Archiva una sucursal en el sistema.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<ArchiveBranchCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new ArchiveBranchCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
