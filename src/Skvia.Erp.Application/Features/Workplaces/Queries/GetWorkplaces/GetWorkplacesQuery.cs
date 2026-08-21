using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Application.Features.Workplaces.Queries.GetWorkplaces;

public record WorkplaceDto(
    Guid Id,
    string Code,
    string Name,
    string? Address,
    string TimeZoneId,
    double? Latitude,
    double? Longitude,
    double GeofenceRadiusMeters,
    bool RequirePhotoForMobile);

public record GetWorkplacesQuery() : IQuery<ErrorOr<IReadOnlyList<WorkplaceDto>>>;


