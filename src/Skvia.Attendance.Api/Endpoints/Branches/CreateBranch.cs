using Skvia.Attendance.Api.Endpoints.Branches.Requests;
using Skvia.Attendance.Api.Endpoints.Branches.Responses;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Branches.Commands.CreateBranch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public sealed class CreateBranch : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithName(nameof(CreateBranch))
            .WithSummary("Crear sucursal")
            .WithDescription("Crea una nueva sucursal/sede en el sistema.")
            .Produces<CreateBranchResponse>(StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        CreateBranchRequest request,
        ICommandHandler<CreateBranchCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateBranchCommand(request.Code, request.Name, request.Address, request.TardinessToleranceMinutes);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            branchId => TypedResults.Created($"/api/v1/branches/{branchId}", new CreateBranchResponse(branchId)),
            errors => errors.ToProblem());
    }
}
