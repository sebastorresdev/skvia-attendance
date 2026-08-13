using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.DTOs;

public record ResolvedScheduleDayDto(
    Guid EmployeeId,
    DateOnly Date,
    ScheduleDayType DayType,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    bool HasBreak,
    TimeOnly? BreakStartTime,
    TimeOnly? BreakEndTime,
    Guid? ScheduleId,
    string? ScheduleCode,
    string? ScheduleDescription,
    bool IsException,
    Guid? ExceptionId,
    string? ExceptionReason);
