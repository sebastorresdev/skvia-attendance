using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Domain.Branches;

public class KioskDevice : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public Guid BranchId { get; private set; }
    public Branch Branch { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private KioskDevice() { }

    public static KioskDevice Create(string name, Guid branchId, string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentOutOfRangeException.ThrowIfEqual(branchId, Guid.Empty);

        return new KioskDevice
        {
            Name = name.Trim(),
            BranchId = branchId,
            Token = token,
            IsActive = true
        };
    }

    public void Revoke()
    {
        IsActive = false;
    }
}
