using Skvia.Attendance.Domain.Attendances;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.EndBreak;

public record EndBreakCommand(
    string EmployeeIdentifier,
    Guid BranchId,
    string PhotoUrl,
    AttendanceSource Source,
    double? Latitude = null,
    double? Longitude = null) : ICommand<ErrorOr<Success>>;
