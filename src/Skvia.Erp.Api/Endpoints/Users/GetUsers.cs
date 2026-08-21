using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Users.DTOs;
using Skvia.Erp.Application.Features.Users.Queries.GetUsers;

namespace Skvia.Erp.Api.Endpoints.Users;

public sealed class GetUsers : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithName(nameof(GetUsers))
            .WithSummary("Obtener usuarios")
            .WithDescription("Obtiene el listado completo de usuarios registrados en el sistema.")
            .Produces<List<UserResponse>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        IQueryHandler<GetUsersQuery, ErrorOr<List<UserResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery();
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}



