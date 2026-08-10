namespace Skvia.Attendance.Application.Common.Interfaces;

public record KioskPairingState(
    string Code,
    bool IsApproved,
    string? Token,
    Guid? WorkplaceId,
    DateTime ExpiresAt);

public interface IKioskPairingService
{
    string GeneratePairingCode();
    KioskPairingState? GetPairingState(string code);
    bool ApprovePairingCode(string code, string token, Guid workplaceId);
}
