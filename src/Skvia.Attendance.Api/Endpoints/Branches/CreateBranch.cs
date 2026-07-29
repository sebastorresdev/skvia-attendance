using Skvia.Attendance.Application.Features.Branches.Commands.CreateBranch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public class CreateBranch : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithSummary("Crear sucursal")
            .Produces<Guid>(StatusCodes.Status201Created);

    private static async Task<IResult> Handle(
        CreateBranchCommand command,
        ICommandHandler<CreateBranchCommand, ErrorOr<Guid>> handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(command, ct);

        return result.Match(
            branchId => Results.Created($"/branches/{branchId}", branchId),
            ResultExtensions.ToProblem);
    }
}
