using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.KioskDevices.Commands.AuthorizeDevice;

namespace Skvia.Erp.Api.Endpoints.KioskDevices;

public sealed class AuthorizeDevice : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/authorize", Handle)
            .WithName(nameof(AuthorizeDevice))
            .WithSummary("Autorizar un dispositivo Kiosko")
            .WithDescription("Registra y autoriza un dispositivo y retorna su token y código de vinculación.")
            .Produces<AuthorizeDeviceResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        AuthorizeDeviceCommand request,
        ICommandHandler<AuthorizeDeviceCommand, ErrorOr<AuthorizeDeviceResult>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);

        return result.Match(
            res => TypedResults.Ok(new AuthorizeDeviceResponse(
                res.DeviceId,
                res.Name,
                res.WorkplaceId,
                res.WorkplaceName,
                res.Token,
                res.PairingCode,
                res.ExpiresAt)),
            errors => errors.ToProblem());
    }
}

public record AuthorizeDeviceResponse(
    Guid DeviceId,
    string Name,
    Guid WorkplaceId,
    string WorkplaceName,
    string Token,
    string PairingCode,
    DateTime ExpiresAt);




