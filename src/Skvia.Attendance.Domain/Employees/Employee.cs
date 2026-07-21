using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Domain.Employees;

public class Employee : BaseAuditableEntity
{
    public string Code { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public DocumentType DocumentType { get; private set; }
    public string DocumentNumber { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Position { get; private set; }
    public string? Department { get; private set; }
    public DateTimeOffset HireDate { get; private set; }
    public string? PhotoUrl { get; private set; }

    private readonly List<EmployeeSchedule> _employeeSchedules = [];
    public IReadOnlyCollection<EmployeeSchedule> EmployeeSchedules => _employeeSchedules.AsReadOnly();

    private Employee() { }

    public static Employee Create(
        string code,
        string firstName,
        string lastName,
        DocumentType documentType,
        string documentNumber,
        DateTimeOffset hireDate,
        string? email = null,
        string? phone = null,
        string? position = null,
        string? department = null,
        string? photoUrl = null)
    {
        var employee = new Employee();

        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(documentNumber);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(code.Length, EmployeeConstants.CodeMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(firstName.Length, EmployeeConstants.FirstNameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(lastName.Length, EmployeeConstants.LastNameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(documentNumber.Length, EmployeeConstants.DocumentNumberMaxLength);

        employee.Code = code.Trim().ToUpper();
        employee.FirstName = firstName.Trim();
        employee.LastName = lastName.Trim();
        employee.DocumentType = documentType;
        employee.DocumentNumber = documentNumber.Trim();
        employee.HireDate = hireDate;
        employee.Email = email?.Trim();
        employee.Phone = phone?.Trim();
        employee.Position = position?.Trim();
        employee.Department = department?.Trim();
        employee.PhotoUrl = photoUrl?.Trim();

        return employee;
    }

    public void Update(
        string code,
        string firstName,
        string lastName,
        DocumentType documentType,
        string documentNumber,
        DateTimeOffset hireDate,
        string? email = null,
        string? phone = null,
        string? position = null,
        string? department = null,
        string? photoUrl = null)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(documentNumber);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(code.Length, EmployeeConstants.CodeMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(firstName.Length, EmployeeConstants.FirstNameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(lastName.Length, EmployeeConstants.LastNameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(documentNumber.Length, EmployeeConstants.DocumentNumberMaxLength);

        Code = code.Trim().ToUpper();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DocumentType = documentType;
        DocumentNumber = documentNumber.Trim();
        HireDate = hireDate;
        Email = email?.Trim();
        Phone = phone?.Trim();
        Position = position?.Trim();
        Department = department?.Trim();
        PhotoUrl = photoUrl?.Trim();
    }

    public void UpdateProfile(string? phone, string? position, string? department, string? photoUrl)
    {
        Phone = phone?.Trim();
        Position = position?.Trim();
        Department = department?.Trim();
        PhotoUrl = photoUrl?.Trim();
    }

    public void AddEmployeeSchedule(EmployeeSchedule employeeSchedule)
    {
        _employeeSchedules.Add(employeeSchedule);
    }
}
