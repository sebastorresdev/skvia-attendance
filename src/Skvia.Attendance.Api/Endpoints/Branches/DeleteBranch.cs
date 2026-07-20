using Skvia.Attendance.Application.Branches.Commands.DeleteBranch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public class DeleteBranch : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/{id:guid}", Handle)
            .WithSummary("Eliminar sede")
            .Produces(StatusCodes.Status204NoContent);

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<DeleteBranchCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBranchCommand(id);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => Results.NoContent(),
            ResultExtensions.ToProblem);
    }
}
