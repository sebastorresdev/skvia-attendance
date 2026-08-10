using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Branches.Commands.UpdateBranch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public sealed class UpdateBranch : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{id:guid}", Handle)
            .WithName(nameof(UpdateBranch))
            .WithSummary("Actualizar sucursal")
            .WithDescription("Modifica los datos de una tienda/sucursal existente.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        Guid id,
        UpdateBranchRequest request,
        ICommandHandler<UpdateBranchCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBranchCommand(id, request.Code, request.Name, request.Address);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public record UpdateBranchRequest(string Code, string Name, string? Address);
