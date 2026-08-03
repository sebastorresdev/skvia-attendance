using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Domain.Employees;

public class Employee : BaseAuditableEntity
{
    public string Code { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public DocumentIdentifier DocumentIdentifier { get; private set; } = null!;
    public Email? Email { get; private set; }
    public Phone? Phone { get; private set; }
    public string? Position { get; private set; }
    public string? Department { get; private set; }
    public DateTimeOffset HireDate { get; private set; }
    public string? PhotoUrl { get; private set; }
    public Guid? MainBranchId { get; private set; }
    public EmployeeStatus Status { get; private set; }

    private readonly List<EmployeeSchedule> _employeeSchedules = [];
    public IReadOnlyCollection<EmployeeSchedule> EmployeeSchedules => _employeeSchedules.AsReadOnly();

    private Employee() { }

    public static Employee Create(
        string code,
        string firstName,
        string lastName,
        DocumentIdentifier documentIdentifier,
        DateTimeOffset hireDate,
        string? email = null,
        string? phone = null,
        string? position = null,
        string? department = null,
        string? photoUrl = null,
        Guid? mainBranchId = null)
    {
        var employee = new Employee();

        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(code.Length, EmployeeConstants.CodeMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(firstName.Length, EmployeeConstants.FirstNameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(lastName.Length, EmployeeConstants.LastNameMaxLength);

        employee.Code = code.Trim().ToUpper();
        employee.FirstName = firstName.Trim();
        employee.LastName = lastName.Trim();
        employee.DocumentIdentifier = documentIdentifier;
        employee.HireDate = hireDate;
        employee.Email = email != null ? Employees.Email.Create(email) : null;
        employee.Phone = phone != null ? Employees.Phone.Create(phone) : null;
        employee.Position = position?.Trim();
        employee.Department = department?.Trim();
        employee.PhotoUrl = photoUrl?.Trim();
        employee.MainBranchId = mainBranchId;
        employee.Status = EmployeeStatus.Active;

        return employee;
    }

    public void Update(
        string code,
        string firstName,
        string lastName,
        DocumentIdentifier documentIdentifier,
        DateTimeOffset hireDate,
        string? email = null,
        string? phone = null,
        string? position = null,
        string? department = null,
        string? photoUrl = null,
        Guid? mainBranchId = null)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(code.Length, EmployeeConstants.CodeMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(firstName.Length, EmployeeConstants.FirstNameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(lastName.Length, EmployeeConstants.LastNameMaxLength);

        Code = code.Trim().ToUpper();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DocumentIdentifier = documentIdentifier;
        HireDate = hireDate;
        Email = email != null ? Employees.Email.Create(email) : null;
        Phone = phone != null ? Employees.Phone.Create(phone) : null;
        Position = position?.Trim();
        Department = department?.Trim();
        PhotoUrl = photoUrl?.Trim();
        MainBranchId = mainBranchId;
    }

    public void UpdateProfile(string? phone, string? position, string? department, string? photoUrl)
    {
        Phone = phone != null ? Employees.Phone.Create(phone) : null;
        Position = position?.Trim();
        Department = department?.Trim();
        PhotoUrl = photoUrl?.Trim();
    }

    public void ChangeStatus(EmployeeStatus newStatus)
    {
        Status = newStatus;
    }

    public void AddEmployeeSchedule(EmployeeSchedule employeeSchedule)
    {
        _employeeSchedules.Add(employeeSchedule);
    }
}
