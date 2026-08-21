using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.KioskDevices.Queries.GetDevices;

namespace Skvia.Erp.Api.Endpoints.KioskDevices;

public sealed class GetDevices : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithName(nameof(GetDevices))
            .WithSummary("Listar dispositivos kiosko")
            .WithDescription("Obtiene la lista de dispositivos kiosko registrados.")
            .Produces<IReadOnlyList<KioskDeviceDto>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        IQueryHandler<GetDevicesQuery, ErrorOr<IReadOnlyList<KioskDeviceDto>>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetDevicesQuery(), cancellationToken);

        return result.Match(
            devices => TypedResults.Ok(devices),
            errors => errors.ToProblem());
    }
}



