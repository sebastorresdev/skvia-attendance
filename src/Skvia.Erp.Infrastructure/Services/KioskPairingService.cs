using System.Collections.Concurrent;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Infrastructure.Services;

public class KioskPairingService : IKioskPairingService
{
    private static readonly ConcurrentDictionary<string, KioskPairingState> _pairingCodes = new();
    private static readonly Random _random = new();

    public string RegisterPairingCode(
        Guid? deviceId,
        string name,
        Guid workplaceId,
        string? workplaceName,
        string token,
        string? existingCode = null,
        DateTime? existingExpiresAt = null)
    {
        var now = DateTime.UtcNow;

        // Cleanup expired codes
        foreach (var key in _pairingCodes.Keys)
        {
            if (_pairingCodes.TryGetValue(key, out var s) && s.ExpiresAt < now)
            {
                _pairingCodes.TryRemove(key, out _);
            }
        }

        // Remove any existing active pairing code for the same deviceId if present
        if (deviceId.HasValue)
        {
            foreach (var kvp in _pairingCodes)
            {
                if (kvp.Value.DeviceId == deviceId.Value)
                {
                    _pairingCodes.TryRemove(kvp.Key, out _);
                }
            }
        }

        string code;
        DateTime expiresAt;

        if (!string.IsNullOrWhiteSpace(existingCode) && existingExpiresAt.HasValue && existingExpiresAt.Value > now)
        {
            code = existingCode;
            expiresAt = existingExpiresAt.Value;
        }
        else
        {
            do
            {
                code = _random.Next(100000, 999999).ToString();
            } while (_pairingCodes.ContainsKey(code));
            expiresAt = now.AddMinutes(30);
        }

        var newState = new KioskPairingState(
            Code: code,
            DeviceId: deviceId,
            Name: name,
            WorkplaceId: workplaceId,
            WorkplaceName: workplaceName,
            Token: token,
            ExpiresAt: expiresAt);

        _pairingCodes[code] = newState;
        return code;
    }

    public KioskPairingState? ClaimPairingCode(string code)
    {
        var cleanCode = code.Replace("-", "").Replace(" ", "").Trim();
        if (_pairingCodes.TryGetValue(cleanCode, out var state))
        {
            if (state.ExpiresAt < DateTime.UtcNow)
            {
                _pairingCodes.TryRemove(cleanCode, out _);
                return null;
            }

            _pairingCodes.TryRemove(cleanCode, out _);
            return state;
        }

        return null;
    }

    public KioskPairingState? GetPairingState(string code)
    {
        var cleanCode = code.Replace("-", "").Replace(" ", "").Trim();
        if (_pairingCodes.TryGetValue(cleanCode, out var state))
        {
            if (state.ExpiresAt < DateTime.UtcNow)
            {
                _pairingCodes.TryRemove(cleanCode, out _);
                return null;
            }
            return state;
        }

        return null;
    }
}

