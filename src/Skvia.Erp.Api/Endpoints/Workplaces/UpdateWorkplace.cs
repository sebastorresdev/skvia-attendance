using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.Workplaces.Commands.UpdateWorkplace;

namespace Skvia.Erp.Api.Endpoints.Workplaces;

public sealed class UpdateWorkplace : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{id:guid}", Handle)
            .WithName(nameof(UpdateWorkplace))
            .WithSummary("Actualizar un lugar de marcación")
            .WithDescription("Modifica los datos de un lugar de marcación.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromBody] UpdateWorkplaceRequest request,
        ICommandHandler<UpdateWorkplaceCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateWorkplaceCommand(
            id,
            request.Code,
            request.Name,
            request.Address,
            request.TimeZoneId ?? "America/Lima",
            request.Latitude,
            request.Longitude,
            request.GeofenceRadiusMeters,
            request.RequirePhotoForMobile);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public record UpdateWorkplaceRequest(
    string Code,
    string Name,
    string? Address,
    string? TimeZoneId,
    double? Latitude,
    double? Longitude,
    double GeofenceRadiusMeters,
    bool RequirePhotoForMobile = true);



