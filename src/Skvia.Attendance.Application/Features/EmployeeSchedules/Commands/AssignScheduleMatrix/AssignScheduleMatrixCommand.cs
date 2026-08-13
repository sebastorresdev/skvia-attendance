using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.AssignScheduleMatrix;

public record ScheduleMatrixCellItem(
    Guid EmployeeId,
    DateOnly Date,
    ScheduleDayType DayType,
    Guid? CustomScheduleId,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Reason);

public record AssignScheduleMatrixCommand(
    List<ScheduleMatrixCellItem> Cells) : ICommand<ErrorOr<Success>>;
