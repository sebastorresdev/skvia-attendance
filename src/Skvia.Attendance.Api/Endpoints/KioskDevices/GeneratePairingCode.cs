using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Api.Endpoints.KioskDevices;

public sealed class GeneratePairingCode : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/pairing-code", Handle)
            .WithName(nameof(GeneratePairingCode))
            .WithSummary("Generar código PIN de vinculación para Kiosko")
            .Produces<GeneratePairingCodeResponse>(StatusCodes.Status200OK)
            .AllowAnonymous();

    private static IResult Handle(IKioskPairingService pairingService)
    {
        var code = pairingService.GeneratePairingCode();
        return TypedResults.Ok(new GeneratePairingCodeResponse(code));
    }
}

public record GeneratePairingCodeResponse(string Code);
