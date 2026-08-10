using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Domain.Departments;

public class Department : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    private Department() { }

    public static Department Create(string name, string? description = null)
    {
        ArgumentNullException.ThrowIfNull(name);

        return new Department
        {
            Name = name.Trim(),
            Description = description?.Trim()
        };
    }

    public void Update(string name, string? description)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name.Trim();
        Description = description?.Trim();
    }
}
