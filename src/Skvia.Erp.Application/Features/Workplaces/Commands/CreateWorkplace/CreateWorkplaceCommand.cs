using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Security;

namespace Skvia.Erp.Application.Features.Workplaces.Commands.CreateWorkplace;

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


