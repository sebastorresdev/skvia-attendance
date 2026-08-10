using Skvia.Attendance.Domain.Common;

using Skvia.Attendance.Domain.Workplaces;

namespace Skvia.Attendance.Domain.Kiosks;

public class KioskDevice : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public Guid WorkplaceId { get; private set; }
    public Workplace Workplace { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private KioskDevice() { }

    public static KioskDevice Create(string name, Guid workplaceId, string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentOutOfRangeException.ThrowIfEqual(workplaceId, Guid.Empty);

        return new KioskDevice
        {
            Name = name.Trim(),
            WorkplaceId = workplaceId,
            Token = token,
            IsActive = true
        };
    }

    public void Revoke()
    {
        IsActive = false;
    }
}
