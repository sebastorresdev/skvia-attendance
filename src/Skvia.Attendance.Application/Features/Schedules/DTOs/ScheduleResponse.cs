namespace Skvia.Attendance.Application.Features.Schedules.DTOs;

public record ScheduleResponse(
    Guid Id,
    string Code,
    string Description,
    string TimeZoneId,
    bool HasBreak,
    TimeOnly? BreakStartTime,
    TimeOnly? BreakEndTime,
    TimeOnly DefaultStartTime,
    TimeOnly DefaultEndTime);
