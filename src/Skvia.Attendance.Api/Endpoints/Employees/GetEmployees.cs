using Skvia.Attendance.Application.Employees.DTOs;
using Skvia.Attendance.Application.Employees.Queries.GetEmployees;
using Skvia.Attendance.Contracts.Employees;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public class GetEmployees : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithSummary("Obtener Empleados")
            .Produces<List<GetEmployeeResponse>>();
    }
    
    private static async Task<IResult> Handle(
        IQueryHandler<GetEmployeesQuery, ErrorOr<List<GetEmployeeResponse>>> handler,
        CancellationToken ct)
    {
        var query = new GetEmployeesQuery();

        var result = await handler.HandleAsync(query, ct);

        return result.Match(
            res => TypedResults.Ok(res.Select(e => e.ToResponse()).ToList()),
            ResultExtensions.ToProblem);
    }
}

public static class GetEmployeesExtensions
{
    public static EmployeeResponse ToResponse(this GetEmployeeResponse result)
    {
        return new EmployeeResponse(result.Id, result.Code, result.FirstName, result.LastName, result.Department, result.PhotoUrl);
    }
}
