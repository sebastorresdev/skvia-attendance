using Skvia.Attendance.Application.Features.Employees.Commands.UpdateEmployee;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public class UpdateEmployee : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", Handle)
            .WithSummary("Actualizar Empleado")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest); // Add bad request for mismatching IDs
    }

    private static async Task<IResult> Handle(
        Guid id,
        UpdateEmployeeCommand command,
        ICommandHandler<UpdateEmployeeCommand, ErrorOr<Success>> handler,
        CancellationToken ct)
    {
        if (id != command.Id)
        {
            return TypedResults.BadRequest(new { Message = "Route ID and command ID do not match." });
        }

        var result = await handler.HandleAsync(command, ct);

        return result.Match(
            _ => TypedResults.NoContent(),
            ResultExtensions.ToProblem);
    }
}
