namespace Skvia.Attendance.Application.Features.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(
    string Code,
    string FirstName,
    string LastName,
    int DocumentType,
    string DocumentNumber,
    DateTimeOffset HireDate,
    string? Email = null,
    string? Phone = null,
    string? Position = null,
    string? Department = null,
    string? PhotoUrl = null) : ICommand<ErrorOr<Guid>>;
