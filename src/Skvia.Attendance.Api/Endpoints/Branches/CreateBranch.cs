using Skvia.Attendance.Application.Branches.Commands.CreateBranch;
using Skvia.Attendance.Contracts.Branch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public class CreateBranch : IEndpoint
{
    public record CreateBranchRequest(string Code, string Name, string? Address);

    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithSummary("Crear sucursal")
            .Produces<CreateBranchResponse>(StatusCodes.Status201Created);

    private static async Task<IResult> Handle(
        CreateBranchRequest request,
        ICommandHandler<CreateBranchCommand, ErrorOr<Guid>> handler,
        CancellationToken ct)
    {
        var command = new CreateBranchCommand(request.Code, request.Name, request.Address);

        var result = await handler.HandleAsync(command, ct);

        return result.Match(
            branchId => Results.Created($"/branches/{branchId}", branchId),
            ResultExtensions.ToProblem);
    }
}
