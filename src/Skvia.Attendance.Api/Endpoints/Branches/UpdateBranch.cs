using Skvia.Attendance.Application.Features.Branches.Commands.UpdateBranch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public class UpdateBranch : IEndpoint
{
    public record UpdateBranchRequest(string Code, string Name, string TimeZoneId, string? Address);
    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{id:guid}", Handle)
            .WithSummary("Actualizar sucursal")
            .WithDescription("Modifica los datos de una tienda/sucursal existente.")
            .Produces(StatusCodes.Status204NoContent);

    private static async Task<IResult> Handle(
        Guid id,
        UpdateBranchRequest request,
        ICommandHandler<UpdateBranchCommand, ErrorOr<Success>> handler,
        CancellationToken ct)
    {
        var command = new UpdateBranchCommand(id, request.Name, request.Code, request.Address);

        var result = await handler.HandleAsync(command, ct);

        return result.Match(
            _ => Results.NoContent(),
            ResultExtensions.ToProblem);
    }
}
