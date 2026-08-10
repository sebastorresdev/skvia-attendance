using Skvia.Attendance.Application.Common.Interfaces;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Attendances.Queries.GetAttendances;

public record GetAttendancesQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    Guid? WorkplaceId = null,
    string? EmployeeSearch = null,
    Guid? EmployeeId = null,
    string? StatusFilter = null) : IQuery<ErrorOr<List<AttendanceResponse>>>;
