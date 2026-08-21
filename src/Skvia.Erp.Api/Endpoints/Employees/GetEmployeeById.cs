using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Employees.DTOs;
using Skvia.Erp.Application.Features.Employees.Queries.GetEmployeeById;

namespace Skvia.Erp.Api.Endpoints.Employees;

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



