using Skvia.Attendance.Application.Features.Branches.Commands.UnarchiveBranch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public class UnarchiveBranch : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}/unarchive", Handle)
            .WithSummary("Desarchiva sucursal")
            .WithDescription("Desarchiv una sucursal en el sistema.")
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<UnarchiveBranchCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UnarchiveBranchCommand(id);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => Results.NoContent(),
            ResultExtensions.ToProblem);
    }
}
