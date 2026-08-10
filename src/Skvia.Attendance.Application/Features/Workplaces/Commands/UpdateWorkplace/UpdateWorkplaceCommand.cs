using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Common.Security;

namespace Skvia.Attendance.Application.Features.Workplaces.Commands.UpdateWorkplace;

[AuthorizeCommand(Permissions = Permission.Workplace.Update)]
public record UpdateWorkplaceCommand(
    Guid Id,
    string Code,
    string Name,
    string? Address,
    string TimeZoneId,
    double? Latitude,
    double? Longitude,
    double GeofenceRadiusMeters,
    bool RequirePhotoForMobile) : ICommand<ErrorOr<Success>>;
