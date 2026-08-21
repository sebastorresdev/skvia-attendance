using Skvia.Erp.Domain.Common;
using Skvia.Erp.Domain.Workplaces;

namespace Skvia.Erp.Domain.Kiosks;

public class KioskDevice : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public Guid WorkplaceId { get; private set; }
    public Workplace Workplace { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public KioskDeviceStatus Status { get; private set; }
    public string? PairingCode { get; private set; }
    public DateTime? PairingCodeExpiresAt { get; private set; }
    public DateTime? LinkedAt { get; private set; }

    public bool IsActive => Status == KioskDeviceStatus.Linked;

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
            Status = KioskDeviceStatus.Pending
        };
    }

    public void SetPairingCode(string code, DateTime expiresAt)
    {
        if (Status != KioskDeviceStatus.Pending)
        {
            throw new InvalidOperationException("Solo se pueden generar códigos de vinculación para kioskos en estado Pendiente.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        PairingCode = code;
        PairingCodeExpiresAt = expiresAt;
    }

    public void MarkAsLinked()
    {
        Status = KioskDeviceStatus.Linked;
        LinkedAt = DateTime.UtcNow;
        PairingCode = null;
        PairingCodeExpiresAt = null;
    }

    public void Unlink(string newToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newToken);
        Status = KioskDeviceStatus.Pending;
        Token = newToken;
        LinkedAt = null;
        PairingCode = null;
        PairingCodeExpiresAt = null;
    }

    public void Disable()
    {
        if (Status != KioskDeviceStatus.Linked)
        {
            throw new InvalidOperationException("Solo se pueden inhabilitar kioskos que se encuentren actualmente Vinculados.");
        }

        Status = KioskDeviceStatus.Revoked;
        PairingCode = null;
        PairingCodeExpiresAt = null;
    }

    public void ReactivateFromDisabled()
    {
        if (Status != KioskDeviceStatus.Revoked)
        {
            throw new InvalidOperationException("Solo se pueden reactivar kioskos que se encuentren en estado Inactivo / Deshabilitado.");
        }

        Status = KioskDeviceStatus.Linked;
    }

    public bool IsPairingCodeValid()
    {
        return Status == KioskDeviceStatus.Pending &&
               !string.IsNullOrWhiteSpace(PairingCode) &&
               PairingCodeExpiresAt.HasValue &&
               PairingCodeExpiresAt.Value > DateTime.UtcNow;
    }
}

