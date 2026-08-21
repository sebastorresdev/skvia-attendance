namespace Skvia.Erp.Application.Common.Interfaces;

public record KioskPairingState(
    string Code,
    Guid? DeviceId,
    string Name,
    Guid WorkplaceId,
    string? WorkplaceName,
    string Token,
    DateTime ExpiresAt);

public interface IKioskPairingService
{
    string RegisterPairingCode(
        Guid? deviceId,
        string name,
        Guid workplaceId,
        string? workplaceName,
        string token,
        string? existingCode = null,
        DateTime? existingExpiresAt = null);

    KioskPairingState? ClaimPairingCode(string code);
    KioskPairingState? GetPairingState(string code);
}

