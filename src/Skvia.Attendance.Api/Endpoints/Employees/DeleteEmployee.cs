using Skvia.Attendance.Application.Features.Employees.Commands.DeleteEmployee;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public class DeleteEmployee : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", Handle)
            .WithSummary("Eliminar Empleado")
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<DeleteEmployeeCommand, ErrorOr<Success>> handler,
        CancellationToken ct)
    {
        var command = new DeleteEmployeeCommand(id);

        var result = await handler.HandleAsync(command, ct);

        return result.Match(
            _ => TypedResults.NoContent(),
            ResultExtensions.ToProblem);
    }
}
