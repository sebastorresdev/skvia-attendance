using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Users.Commands.CreateUser;

namespace Skvia.Erp.Api.Endpoints.Users;

public sealed class CreateUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithName(nameof(CreateUser))
            .WithSummary("Crear usuario")
            .WithDescription("Crea un nuevo usuario en el sistema.")
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        CreateUserRequest request,
        ICommandHandler<CreateUserCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            request.UserName,
            request.Password,
            request.Email,
            request.DisplayName,
            request.PhoneNumber,
            request.PhotoUrl,
            request.BranchIds,
            request.RoleIds);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            userId => TypedResults.Created($"/api/v1/users/{userId}", new CreateUserResponse(userId)),
            errors => errors.ToProblem());
    }
}

public record CreateUserRequest(
    string UserName,
    string Password,
    string Email,
    string? DisplayName,
    string? PhoneNumber,
    string? PhotoUrl,
    List<Guid> BranchIds,
    List<Guid> RoleIds);

public record CreateUserResponse(Guid Id);



