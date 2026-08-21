using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.Departments.Queries.GetDepartments;
using Skvia.Erp.Application.Features.Departments.DTOs;
using ErrorOr;

namespace Skvia.Erp.Api.Endpoints.Departments;

public sealed class GetDepartments : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithName(nameof(GetDepartments))
            .WithSummary("Obtener departamentos")
            .WithDescription("Obtiene la lista de todos los departamentos.")
            .Produces<List<DepartmentResponse>>(StatusCodes.Status200OK);

    private static async Task<IResult> Handle(
        IQueryHandler<GetDepartmentsQuery, ErrorOr<List<DepartmentResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetDepartmentsQuery();
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}


