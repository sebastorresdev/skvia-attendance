using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.Workplaces.Queries.GetWorkplaces;

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
