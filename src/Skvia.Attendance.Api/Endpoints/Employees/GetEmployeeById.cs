using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Employees.DTOs;
using Skvia.Attendance.Application.Features.Employees.Queries.GetEmployeeById;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public sealed class GetEmployeeById : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{id:guid}", Handle)
            .WithName(nameof(GetEmployeeById))
            .WithSummary("Obtener empleado por ID")
            .WithDescription("Obtiene los detalles de un empleado específico mediante su identificador único.")
            .Produces<EmployeeDetailResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        IQueryHandler<GetEmployeeByIdQuery, ErrorOr<EmployeeDetailResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetEmployeeByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}
