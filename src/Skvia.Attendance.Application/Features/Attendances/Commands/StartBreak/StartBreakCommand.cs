using Skvia.Attendance.Domain.Attendances;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.StartBreak;

public record StartBreakCommand(
    string EmployeeIdentifier,
    Guid BranchId,
    string PhotoUrl,
    AttendanceSource Source,
    double? Latitude = null,
    double? Longitude = null) : ICommand<ErrorOr<Success>>;
