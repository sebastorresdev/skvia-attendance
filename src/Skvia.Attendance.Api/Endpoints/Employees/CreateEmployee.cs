using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Employees.Commands.CreateEmployee;
using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public sealed class CreateEmployee : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithName(nameof(CreateEmployee))
            .WithSummary("Crear empleado")
            .WithDescription("Permite registrar un nuevo empleado en el sistema.")
            .Produces<CreateEmployeeResponse>(StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

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
            request.DepartmentId,
            request.PhotoUrl,
            request.MainBranchId,
            request.MobileCheckInEnabled,
            request.ApplicationUserId,
            request.RequireFourPointAttendance,
            request.IsAttendanceTracked,
            request.AutoCompleteClockOut,
            request.TardinessToleranceMinutes,
            request.AllowedWorkplaceIds);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            employeeId => TypedResults.Created($"/api/v1/employees/{employeeId}", new CreateEmployeeResponse(employeeId)),
            errors => errors.ToProblem());
    }
}

public record CreateEmployeeRequest(
    string Code,
    string FirstName,
    string LastName,
    DocumentType DocumentType,
    string DocumentNumber,
    DateTimeOffset HireDate,
    string? Email = null,
    string? Phone = null,
    string? Position = null,
    Guid? DepartmentId = null,
    string? PhotoUrl = null,
    Guid? MainBranchId = null,
    bool MobileCheckInEnabled = false,
    string? ApplicationUserId = null,
    bool RequireFourPointAttendance = false,
    bool IsAttendanceTracked = true,
    bool AutoCompleteClockOut = false,
    int TardinessToleranceMinutes = 0,
    List<Guid>? AllowedWorkplaceIds = null);
public record CreateEmployeeResponse(Guid Id);
