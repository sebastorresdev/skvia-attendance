using Skvia.Attendance.Domain.Attendances;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.StartBreak;

public record StartBreakCommand(
    string EmployeeIdentifier,
    Guid WorkplaceId,
    string PhotoUrl,
    AttendanceSource Source,
    double? Latitude = null,
    double? Longitude = null,
    string? DeviceId = null,
    string? Token = null) : ICommand<ErrorOr<Success>>;
