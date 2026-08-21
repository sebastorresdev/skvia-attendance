using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Domain.Attendances;
using ErrorOr;

namespace Skvia.Erp.Application.Features.Attendances.Commands.StartBreak;

public record StartBreakCommand(
    string EmployeeIdentifier,
    Guid WorkplaceId,
    string PhotoUrl,
    AttendanceSource Source,
    double? Latitude = null,
    double? Longitude = null,
    string? DeviceId = null,
    string? Token = null) : ICommand<ErrorOr<Success>>;


