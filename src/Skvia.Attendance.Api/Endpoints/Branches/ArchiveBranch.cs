using Skvia.Attendance.Application.Features.Branches.Commands.ArchiveBranch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public class ArchiveBranch : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}/archive", Handle)
            .WithSummary("Archivar sucursal")
            .WithDescription("Archiva una sucursal en el sistema.")
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<ArchiveBranchCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new ArchiveBranchCommand(id);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => Results.NoContent(),
            ResultExtensions.ToProblem);
    }
}
