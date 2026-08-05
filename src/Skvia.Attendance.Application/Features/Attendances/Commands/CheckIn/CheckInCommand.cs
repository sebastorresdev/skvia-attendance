using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Attendances;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.CheckIn;

public record CheckInCommand(
    string EmployeeIdentifier, // Can be DNI or Code
    Guid BranchId,
    string PhotoUrl,
    AttendanceSource Source,
    double? Latitude = null,
    double? Longitude = null,
    string? DeviceId = null,
    string? Token = null) : ICommand<ErrorOr<Success>>;
