using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Users.DTOs;
using Skvia.Erp.Application.Features.Users.Queries.GetUserById;

namespace Skvia.Erp.Api.Endpoints.Users;

public sealed class GetUserById : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{userId:guid}", Handle)
            .WithName(nameof(GetUserById))
            .WithSummary("Obtener usuario por ID")
            .WithDescription("Obtiene los detalles de un usuario específico mediante su identificador único.")
            .Produces<UserDetailResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid userId,
        IQueryHandler<GetUserByIdQuery, ErrorOr<UserDetailResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(userId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}



