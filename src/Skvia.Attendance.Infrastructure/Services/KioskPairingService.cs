using System.Collections.Concurrent;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Infrastructure.Services;

public class KioskPairingService : IKioskPairingService
{
    private static readonly ConcurrentDictionary<string, KioskPairingState> _pairingCodes = new();
    private static readonly Random _random = new();

    public string GeneratePairingCode()
    {
        // Cleanup expired codes
        var now = DateTime.UtcNow;
        foreach (var key in _pairingCodes.Keys)
        {
            if (_pairingCodes.TryGetValue(key, out var state) && state.ExpiresAt < now)
            {
                _pairingCodes.TryRemove(key, out _);
            }
        }

        string code;
        do
        {
            code = _random.Next(100000, 999999).ToString();
        } while (_pairingCodes.ContainsKey(code));

        var newState = new KioskPairingState(
            Code: code,
            IsApproved: false,
            Token: null,
            WorkplaceId: null,
            ExpiresAt: now.AddMinutes(15));

        _pairingCodes[code] = newState;
        return code;
    }

    public KioskPairingState? GetPairingState(string code)
    {
        if (_pairingCodes.TryGetValue(code, out var state))
        {
            if (state.ExpiresAt < DateTime.UtcNow)
            {
                _pairingCodes.TryRemove(code, out _);
                return null;
            }
            return state;
        }
        return null;
    }

    public bool ApprovePairingCode(string code, string token, Guid workplaceId)
    {
        if (_pairingCodes.TryGetValue(code, out var state))
        {
            if (state.ExpiresAt < DateTime.UtcNow)
            {
                _pairingCodes.TryRemove(code, out _);
                return false;
            }

            var approvedState = state with
            {
                IsApproved = true,
                Token = token,
                WorkplaceId = workplaceId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5) // Keep approved state for 5 mins so remote client can retrieve it
            };

            _pairingCodes[code] = approvedState;
            return true;
        }

        return false;
    }
}
