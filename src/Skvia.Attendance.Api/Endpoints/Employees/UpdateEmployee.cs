using Skvia.Attendance.Application.Employees.Commands.UpdateEmployee;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public class UpdateEmployee : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", Handle)
            .WithSummary("Actualizar Empleado")
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        Guid id,
        UpdateEmployeeCommand command,
        ICommandHandler<UpdateEmployeeCommand, ErrorOr<Success>> handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(command, ct);

        return result.Match(
            _ => TypedResults.NoContent(),
            ResultExtensions.ToProblem);
    }
}
