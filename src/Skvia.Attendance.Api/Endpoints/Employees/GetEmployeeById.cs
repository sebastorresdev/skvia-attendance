using Skvia.Attendance.Application.Employees.DTOs;
using Skvia.Attendance.Application.Employees.Queries.GetEmployeeById;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public class GetEmployeeById : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", Handle)
            .WithSummary("Obtener Empleado por Id")
            .Produces<EmployeeDetailResponse>();
    }

    private static async Task<IResult> Handle(
        Guid id,
        IQueryHandler<GetEmployeeByIdQuery, ErrorOr<EmployeeDetailResponse>> handler,
        CancellationToken ct)
    {
        var query = new GetEmployeeByIdQuery(id);

        var result = await handler.HandleAsync(query, ct);

        return result.Match(
            TypedResults.Ok,
            ResultExtensions.ToProblem);
    }
}
