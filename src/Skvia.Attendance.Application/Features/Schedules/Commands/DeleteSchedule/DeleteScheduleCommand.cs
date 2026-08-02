using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.Schedules.Commands.DeleteSchedule;

public record DeleteScheduleCommand(Guid Id) : ICommand<ErrorOr<Success>>;
