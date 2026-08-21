using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Users.Commands.UpdateUser;

namespace Skvia.Erp.Api.Endpoints.Users;

public sealed class UpdateUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{userId:guid}", Handle)
            .WithName(nameof(UpdateUser))
            .WithSummary("Actualizar usuario")
            .WithDescription("Actualiza la información de un usuario existente.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        Guid userId,
        UpdateUserRequest request,
        ICommandHandler<UpdateUserCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(
            userId,
            request.UserName,
            request.IsActive,
            request.Email,
            request.DisplayName,
            request.PhoneNumber,
            request.PhotoUrl,
            request.BranchIds,
            request.RoleIds);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public record UpdateUserRequest(
    string UserName,
    bool IsActive,
    string Email,
    string? DisplayName,
    string? PhoneNumber,
    string? PhotoUrl,
    List<Guid> BranchIds,
    List<Guid> RoleIds);



