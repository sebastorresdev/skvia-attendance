using Skvia.Attendance.Application.Features.Employees.DTOs;
using Skvia.Attendance.Application.Features.Employees.Queries.GetEmployees;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public class GetEmployees : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithSummary("Obtener Empleados")
            .Produces<List<EmployeeResponse>>();
    }

    private static async Task<IResult> Handle(
        IQueryHandler<GetEmployeesQuery, ErrorOr<List<EmployeeResponse>>> handler,
        CancellationToken ct)
    {
        var query = new GetEmployeesQuery();

        var result = await handler.HandleAsync(query, ct);

        return result.Match(
            TypedResults.Ok,
            ResultExtensions.ToProblem);
    }
}
