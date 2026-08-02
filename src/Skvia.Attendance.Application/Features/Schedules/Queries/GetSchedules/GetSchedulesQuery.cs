using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Schedules.DTOs;

namespace Skvia.Attendance.Application.Features.Schedules.Queries.GetSchedules;

public record GetSchedulesQuery : IQuery<ErrorOr<List<ScheduleResponse>>>;
