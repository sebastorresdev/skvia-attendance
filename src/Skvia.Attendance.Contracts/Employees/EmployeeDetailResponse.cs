namespace Skvia.Attendance.Contracts.Employees;

// TODO: Revisar el document type segun la necesidad en la vista
public record EmployeeDetailResponse(
    Guid Id,
    string Code,
    string FirstName,
    string LastName,
    string DocumentType,
    string DocumentNumber,
    string? Email,
    string? Phone,
    string? Position,
    string? Department,
    DateTimeOffset HireDate,
    string? PhotoUrl);
