namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.DeleteScheduleException;

public record DeleteScheduleExceptionCommand(Guid Id) : ICommand<ErrorOr<Success>>;
