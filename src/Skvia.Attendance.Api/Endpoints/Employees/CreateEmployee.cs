using Skvia.Attendance.Api.Endpoints.Employees.Requests;
using Skvia.Attendance.Api.Endpoints.Employees.Responses;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Employees.Commands.CreateEmployee;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public sealed class CreateEmployee : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", Handle)
            .WithName(nameof(CreateEmployee))
            .WithSummary("Crear empleado")
            .WithDescription("Permite registrar un nuevo empleado en el sistema.")
            .Produces<CreateEmployeeResponse>(StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle(
        CreateEmployeeRequest request,
        ICommandHandler<CreateEmployeeCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateEmployeeCommand(
            request.Code,
            request.FirstName,
            request.LastName,
            request.DocumentType,
            request.DocumentNumber,
            request.HireDate,
            request.Email,
            request.Phone,
            request.Position,
            request.Department,
            request.PhotoUrl);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            employeeId => TypedResults.Created($"/api/v1/employees/{employeeId}", new CreateEmployeeResponse(employeeId)),
            errors => errors.ToProblem());
    }
}
