using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Common.Security;

namespace Skvia.Attendance.Application.Features.Workplaces.Commands.CreateWorkplace;

[AuthorizeCommand(Permissions = Permission.Workplace.Create)]
public record CreateWorkplaceCommand(
    string Code,
    string Name,
    string? Address,
    string TimeZoneId,
    double? Latitude,
    double? Longitude,
    double GeofenceRadiusMeters,
    bool RequirePhotoForMobile = true) : ICommand<ErrorOr<Guid>>;
