using Skvia.Attendance.Application.Employees.DTOs;
using Skvia.Attendance.Application.Employees.Queries.GetEmployeeById;
using Skvia.Attendance.Contracts.Employees;
using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public class GetEmployeeById : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", Handle)
            .WithSummary("Obtener Empleado por Id")
            .Produces<GetEmployeeByIdResponse>();
    }

    private static async Task<IResult> Handle(
        Guid id,
        IQueryHandler<GetEmployeeByIdQuery, ErrorOr<GetEmployeeByIdResponse>> handler,
        CancellationToken ct)
    {
        var query = new GetEmployeeByIdQuery(id);

        var result = await handler.HandleAsync(query, ct);

        return result.Match(
            res => TypedResults.Ok(res.ToResponse()),
            ResultExtensions.ToProblem);
    }
}

public static class GetEmployeeByIdExtensions
{
    public static EmployeeDetailResponse ToResponse(
        this GetEmployeeByIdResponse result)
    {
        return new EmployeeDetailResponse(
            result.Id,
            result.Code,
            result.FirstName,
            result.LastName,
            result.DocumentType switch
            {
                DocumentType.Dni => "DNI",
                DocumentType.Ce => "CE",
                DocumentType.Passport => "PASSPORT",

                _ => throw new ArgumentOutOfRangeException(
                    nameof(result.DocumentType),
                    result.DocumentType,
                    "Tipo de documento no soportado.")
            },
            result.DocumentNumber,
            result.Email,
            result.Phone,
            result.Position,
            result.Department,
            result.HireDate,
            result.PhotoUrl);
    }
}
