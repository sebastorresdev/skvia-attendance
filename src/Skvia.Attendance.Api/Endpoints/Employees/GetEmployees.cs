using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Employees.DTOs;
using Skvia.Attendance.Application.Features.Employees.Queries.GetEmployees;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public sealed class GetEmployees : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithName(nameof(GetEmployees))
            .WithSummary("Obtener empleados")
            .WithDescription("Obtiene el listado completo de empleados registrados en el sistema.")
            .Produces<List<EmployeeResponse>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Handle(
        IQueryHandler<GetEmployeesQuery, ErrorOr<List<EmployeeResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetEmployeesQuery();
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}
