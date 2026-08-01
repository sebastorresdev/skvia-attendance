using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Branches.Commands.DeleteBranch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public sealed class DeleteBranch : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/{id:guid}", Handle)
            .WithName(nameof(DeleteBranch))
            .WithSummary("Eliminar sucursal")
            .WithDescription("Elimina permanentemente una sucursal/sede del sistema.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<DeleteBranchCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBranchCommand(id);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
