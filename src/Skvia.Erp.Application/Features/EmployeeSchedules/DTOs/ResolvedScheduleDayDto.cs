using Skvia.Erp.Domain.EmployeeSchedules;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.DTOs;

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

