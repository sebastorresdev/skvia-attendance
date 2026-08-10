using Skvia.Attendance.Domain.Attendances;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.CheckIn;

public record CheckInCommand(
    string EmployeeIdentifier, // Can be DNI or Code
    Guid WorkplaceId,
    string PhotoUrl,
    AttendanceSource Source,
    double? Latitude = null,
    double? Longitude = null,
    string? DeviceId = null,
    string? Token = null) : ICommand<ErrorOr<Success>>;
