using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Domain.Attendances;

namespace Skvia.Erp.Application.Features.Attendances.Commands.CheckIn;

public record CheckInCommand(
    string EmployeeIdentifier, // Can be DNI or Code
    Guid WorkplaceId,
    string PhotoUrl,
    AttendanceSource Source,
    double? Latitude = null,
    double? Longitude = null,
    string? DeviceId = null,
    string? Token = null) : ICommand<ErrorOr<Success>>;


