using Skvia.Attendance.Domain.Attendances;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.EndBreak;

public record EndBreakCommand(
    string EmployeeIdentifier,
    Guid WorkplaceId,
    string PhotoUrl,
    AttendanceSource Source,
    double? Latitude = null,
    double? Longitude = null,
    string? DeviceId = null,
    string? Token = null) : ICommand<ErrorOr<Success>>;
