using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.KioskDevices.Commands.AuthorizeDevice;

namespace Skvia.Attendance.Api.Endpoints.KioskDevices;

public sealed class AuthorizeDevice : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/authorize", Handle)
            .WithName(nameof(AuthorizeDevice))
            .WithSummary("Autorizar un dispositivo Kiosko")
            .WithDescription("Registra y autoriza un dispositivo y retorna su token.")
            .Produces<AuthorizeDeviceResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        AuthorizeDeviceCommand request,
        ICommandHandler<AuthorizeDeviceCommand, ErrorOr<string>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request, cancellationToken);

        return result.Match(
            token => TypedResults.Ok(new AuthorizeDeviceResponse(token)),
            errors => errors.ToProblem());
    }
}

public record AuthorizeDeviceResponse(string Token);
