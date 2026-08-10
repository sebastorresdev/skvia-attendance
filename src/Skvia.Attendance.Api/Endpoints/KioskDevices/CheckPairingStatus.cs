using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Api.Endpoints.KioskDevices;

public sealed class CheckPairingStatus : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/check-pairing/{code}", Handle)
            .WithName(nameof(CheckPairingStatus))
            .WithSummary("Consultar estado de vinculación por PIN")
            .Produces<CheckPairingStatusResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .AllowAnonymous();

    private static IResult Handle(string code, IKioskPairingService pairingService)
    {
        var cleanCode = code.Replace("-", "").Replace(" ", "").Trim();
        var state = pairingService.GetPairingState(cleanCode);
        if (state is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new CheckPairingStatusResponse(
            state.IsApproved,
            state.Token,
            state.WorkplaceId));
    }
}

public record CheckPairingStatusResponse(
    bool IsApproved,
    string? Token,
    Guid? WorkplaceId);
