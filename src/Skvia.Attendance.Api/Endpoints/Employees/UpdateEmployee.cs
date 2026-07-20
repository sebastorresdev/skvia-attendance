using Skvia.Attendance.Application.Employees.Commands.UpdateEmployee;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public class UpdateEmployee : IEndpoint
{
    public record UpdateEmployeeRequest(
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
        string? PhotoUrl = null);
    
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", Handle)
            .WithSummary("Actualizar Empleado")
            .Produces(StatusCodes.Status204NoContent);
    }
    
    private static async Task<IResult> Handle(
        Guid id,
        UpdateEmployeeRequest request,
        ICommandHandler<UpdateEmployeeCommand, ErrorOr<Success>> handler,
        CancellationToken ct)
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

        var result = await handler.HandleAsync(command, ct);

        return result.Match(
            _ => TypedResults.NoContent(),
            ResultExtensions.ToProblem);
    }
}
