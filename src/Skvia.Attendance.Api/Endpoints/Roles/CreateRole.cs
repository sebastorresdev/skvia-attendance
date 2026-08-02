using Skvia.Attendance.Api.Endpoints.Roles.Requests;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Roles.Commands.CreateRole;

namespace Skvia.Attendance.Api.Endpoints.Roles;

public class CreateRole : IEndpoint
{
    public record CreateRoleResponse(Guid Id);

    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithName(nameof(CreateRole))
            .WithSummary("Crear rol")
            .WithDescription("Crea un nuevo rol en el sistema.")
            .Produces<CreateRoleResponse>(StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        CreateRoleRequest request,
        ICommandHandler<CreateRoleCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateRoleCommand(
            request.Name,
            request.Description);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            roleId => TypedResults.Created($"/api/v1/roles/{roleId}", new CreateRoleResponse(roleId)),
            errors => errors.ToProblem());
    }
}
