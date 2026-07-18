using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Domain.Branches;

public class Branch : BaseAuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Address { get; private set; }

    private Branch() { } // EF Core

    public static Branch Create(string code, string name, string? address = null)
    {
        var branch = new Branch();

        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(name);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(code.Length, BranchConstants.CodeMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(name.Length, BranchConstants.NameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(address?.Length ?? 0, BranchConstants.AddressMaxLength);

        branch.Code = code.Trim().ToUpper();
        branch.Name = name.Trim();
        branch.Address = address?.Trim();

        return branch;
    }

    public void Update(string code, string name, string? address = null)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(name);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(code.Length, BranchConstants.CodeMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(name.Length, BranchConstants.NameMaxLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(address?.Length ?? 0, BranchConstants.AddressMaxLength);

        Code = code.Trim().ToUpper();
        Name = name.Trim();
        Address = address?.Trim();
    }

}
