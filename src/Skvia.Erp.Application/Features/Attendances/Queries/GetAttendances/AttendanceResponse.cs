namespace Skvia.Erp.Application.Features.Attendances.Queries.GetAttendances;

public record AttendanceResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeCode,
    Guid WorkplaceId,
    string WorkplaceName,
    DateOnly Date,
    TimeSpan? AssignedStartTime,
    TimeSpan? AssignedEndTime,
    DateTimeOffset? CheckIn,
    DateTimeOffset? CheckOut,
    int TardinessMinutes,
    bool IsLate);

