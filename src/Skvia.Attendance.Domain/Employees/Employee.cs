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
    public Guid? DepartmentId { get; private set; }
    public DateTimeOffset HireDate { get; private set; }
    public string? PhotoUrl { get; private set; }
    public Guid? MainBranchId { get; private set; }
    public EmployeeStatus Status { get; private set; }
    
    public string? ApplicationUserId { get; private set; }
    public bool MobileCheckInEnabled { get; private set; }
    public bool RequireFourPointAttendance { get; private set; } = false;
    public bool IsAttendanceTracked { get; private set; } = true;
    public bool AutoCompleteClockOut { get; private set; } = false;
    public int TardinessToleranceMinutes { get; private set; } = 0;
    public List<Guid> AllowedWorkplaceIds { get; private set; } = [];

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
        Guid? departmentId = null,
        string? photoUrl = null,
        Guid? mainBranchId = null,
        int tardinessToleranceMinutes = 0)
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
        employee.DepartmentId = departmentId;
        employee.PhotoUrl = photoUrl?.Trim();
        employee.MainBranchId = mainBranchId;
        employee.Status = EmployeeStatus.Active;
        employee.IsAttendanceTracked = true;
        employee.TardinessToleranceMinutes = tardinessToleranceMinutes >= 0 ? tardinessToleranceMinutes : 0;

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
        Guid? departmentId = null,
        string? photoUrl = null,
        Guid? mainBranchId = null,
        int? tardinessToleranceMinutes = null)
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
        DepartmentId = departmentId;
        PhotoUrl = photoUrl?.Trim();
        MainBranchId = mainBranchId;
        if (tardinessToleranceMinutes.HasValue && tardinessToleranceMinutes.Value >= 0)
            TardinessToleranceMinutes = tardinessToleranceMinutes.Value;
    }

    public void UpdateProfile(string? phone, string? position, Guid? departmentId, string? photoUrl)
    {
        Phone = phone != null ? Employees.Phone.Create(phone) : null;
        Position = position?.Trim();
        DepartmentId = departmentId;
        PhotoUrl = photoUrl?.Trim();
    }

    public void ChangeStatus(EmployeeStatus newStatus)
    {
        Status = newStatus;
    }

    public void LinkUser(string? applicationUserId)
    {
        ApplicationUserId = applicationUserId;
    }

    public void SetRequireFourPointAttendance(bool require)
    {
        RequireFourPointAttendance = require;
    }

    public void EnableMobileCheckIn(bool enabled)
    {
        if (enabled && !IsAttendanceTracked)
        {
            throw new InvalidOperationException("No se puede habilitar la marcación móvil si no se controla la asistencia.");
        }
        MobileCheckInEnabled = enabled;
    }

    public void SetAttendanceOptions(bool isAttendanceTracked, bool autoCompleteClockOut)
    {
        IsAttendanceTracked = isAttendanceTracked;
        AutoCompleteClockOut = autoCompleteClockOut;
        
        if (!isAttendanceTracked)
        {
            MobileCheckInEnabled = false;
        }
    }

    public void SetAllowedWorkplaceIds(IEnumerable<Guid>? workplaceIds)
    {
        AllowedWorkplaceIds = workplaceIds != null ? workplaceIds.Distinct().ToList() : [];
    }

    public void AddEmployeeSchedule(EmployeeSchedule employeeSchedule)
    {
        _employeeSchedules.Add(employeeSchedule);
    }
}
