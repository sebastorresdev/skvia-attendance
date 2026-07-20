namespace Skvia.Attendance.Application.Employees.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(Guid EmployeeId) : ICommand<ErrorOr<Success>>;
