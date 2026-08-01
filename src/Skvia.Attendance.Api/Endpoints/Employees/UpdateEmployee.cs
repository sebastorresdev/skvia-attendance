using Skvia.Attendance.Api.Endpoints.Employees.Requests;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Employees.Commands.UpdateEmployee;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public sealed class UpdateEmployee : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", Handle)
            .WithName(nameof(UpdateEmployee))
            .WithSummary("Actualizar empleado")
            .WithDescription("Actualiza la información de un empleado existente.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle(
        Guid id,
        UpdateEmployeeRequest request,
        ICommandHandler<UpdateEmployeeCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEmployeeCommand(
            id,
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
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
