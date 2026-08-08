using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Roles.Commands.UpdateRole;

namespace Skvia.Attendance.Api.Endpoints.Roles;

public class UpdateRole : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
       => group.MapPut("/{roleId:guid}", Handle)
           .WithName(nameof(UpdateRole))
           .WithSummary("Actualizar rol")
           .WithDescription("Actualiza la información de un rol existente.")
           .Produces(StatusCodes.Status204NoContent)
           .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
           .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        Guid roleId,
        UpdateRoleRequest request,
        ICommandHandler<UpdateRoleCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRoleCommand(
            roleId,
            request.Name,
            request.Description);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public record UpdateRoleRequest(string Id, string Name, string Description);
