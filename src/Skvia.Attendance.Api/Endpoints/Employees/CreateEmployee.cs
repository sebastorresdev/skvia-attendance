using Skvia.Attendance.Application.Employees.Commands.CreateEmployee;
using Skvia.Attendance.Contracts.Employees;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public class CreateEmployee : IEndpoint
{
    public record CreateEmployeeRequest(
        string Code,
        string FirstName,
        string LastName,
        int DocumentType,
        string DocumentNumber,
        DateTimeOffset HireDate,
        string? Email = null,
        string? Phone = null,
        string? Position = null,
        string? Department = null,
        string? PhotoUrl = null
    );
    
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", Handle)
            .WithSummary("Crear Empleado")
            .Produces<CreateEmployeeResponse>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        CreateEmployeeRequest req,
        ICommandHandler<CreateEmployeeCommand, ErrorOr<Guid>> handler,
        CancellationToken ct)
    {
        var command = new CreateEmployeeCommand(
            req.Code,
            req.FirstName,
            req.LastName,
            req.DocumentType,
            req.DocumentNumber,
            req.HireDate,
            req.Email,
            req.Phone,
            req.Position,
            req.Department,
            req.PhotoUrl);

        var result = await handler.HandleAsync(command, ct);

        return result.Match(
            employeeId => TypedResults.Created($"/api/employees/{employeeId}", employeeId),
            ResultExtensions.ToProblem);
    }
}
