using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.Workplaces.Commands.CreateWorkplace;

namespace Skvia.Erp.Api.Endpoints.Workplaces;

public sealed class CreateWorkplace : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithName(nameof(CreateWorkplace))
            .WithSummary("Crear un nuevo lugar de marcación")
            .WithDescription("Registra un lugar de marcación físico o virtual.")
            .Produces<CreateWorkplaceResponse>(StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        [FromBody] CreateWorkplaceRequest request,
        ICommandHandler<CreateWorkplaceCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateWorkplaceCommand(
            request.Code,
            request.Name,
            request.Address,
            request.TimeZoneId ?? "America/Lima",
            request.Latitude,
            request.Longitude,
            request.GeofenceRadiusMeters > 0 ? request.GeofenceRadiusMeters : 200,
            request.RequirePhotoForMobile);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            id => TypedResults.Created($"/api/v1/workplaces/{id}", new CreateWorkplaceResponse(id)),
            errors => errors.ToProblem());
    }
}

public record CreateWorkplaceRequest(
    string Code,
    string Name,
    string? Address,
    string? TimeZoneId,
    double? Latitude,
    double? Longitude,
    double GeofenceRadiusMeters,
    bool RequirePhotoForMobile = true);

public record CreateWorkplaceResponse(Guid Id);



