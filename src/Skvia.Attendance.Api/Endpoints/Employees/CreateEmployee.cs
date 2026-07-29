using Skvia.Attendance.Application.Features.Employees.Commands.CreateEmployee;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public class CreateEmployee : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", Handle)
            .WithSummary("Crear Empleado")
            .WithDescription("Permite crear un empleado en el sistema.")
            .Produces<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        CreateEmployeeCommand command,
        ICommandHandler<CreateEmployeeCommand, ErrorOr<Guid>> handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(command, ct);

        return result.Match(
            employeeId => TypedResults.Created($"/api/employees/{employeeId}", employeeId),
            ResultExtensions.ToProblem);
    }
}
