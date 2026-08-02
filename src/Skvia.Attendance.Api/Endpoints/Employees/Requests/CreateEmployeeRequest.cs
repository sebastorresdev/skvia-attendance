using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Api.Endpoints.Employees.Requests;

public record CreateEmployeeRequest(
    string Code,
    string FirstName,
    string LastName,
    DocumentType DocumentType,
    string DocumentNumber,
    DateTimeOffset HireDate,
    string? Email = null,
    string? Phone = null,
    string? Position = null,
    string? Department = null,
    string? PhotoUrl = null,
    Guid? MainBranchId = null);
