namespace Skvia.Attendance.Application.Features.Attendances.Queries.GetAttendances;

public record AttendanceResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeCode,
    Guid BranchId,
    string BranchName,
    DateOnly Date,
    TimeSpan? AssignedStartTime,
    TimeSpan? AssignedEndTime,
    DateTimeOffset? CheckIn,
    DateTimeOffset? CheckOut,
    int TardinessMinutes,
    bool IsLate);
