using Skvia.Attendance.Application.Features.EmployeeSchedules.DTOs;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Queries.GetResolvedEmployeeSchedule;

public record GetResolvedEmployeeScheduleQuery(
    Guid EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate) : IQuery<ErrorOr<List<ResolvedScheduleDayDto>>>;
