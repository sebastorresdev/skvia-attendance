using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Security;

namespace Skvia.Erp.Application.Features.Workplaces.Commands.UpdateWorkplace;

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


