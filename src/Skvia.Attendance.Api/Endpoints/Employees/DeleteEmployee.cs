using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Employees.Commands.DeleteEmployee;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public sealed class DeleteEmployee : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/{id:guid}", Handle)
            .WithName(nameof(DeleteEmployee))
            .WithSummary("Eliminar empleado")
            .WithDescription("Elimina permanentemente un empleado del sistema.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<DeleteEmployeeCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteEmployeeCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
