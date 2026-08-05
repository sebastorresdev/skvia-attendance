using Skvia.Attendance.Application.Common.Interfaces;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Attendances.Queries.GetAttendances;

public record GetAttendancesQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    Guid? BranchId = null,
    string? EmployeeSearch = null) : IQuery<ErrorOr<List<AttendanceResponse>>>;
