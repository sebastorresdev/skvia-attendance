using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.DTOs;

public record EmployeeScheduleResponse(
    Guid Id,
    Guid EmployeeId,
    DateOnly Date,
    Guid BranchId,
    string BranchName,
    TimeOnly? AssignedStartTime,
    TimeOnly? AssignedEndTime,
    ScheduleDayType DayType,
    Guid? BaseScheduleId);
