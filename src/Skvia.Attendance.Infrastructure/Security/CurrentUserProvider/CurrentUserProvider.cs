using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using Skvia.Attendance.Application.Common.Constants;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Auth.DTOs;

namespace Skvia.Attendance.Infrastructure.Security.CurrentUserProvider;

public class CurrentUserProvider(IHttpContextAccessor _httpContextAccessor) : ICurrentUserProvider
{
    public CurrentUserResponse GetCurrentUser()
    {
        if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated != true)
        {
            return new CurrentUserResponse(Guid.Empty, [], []);
        }

        ArgumentNullException.ThrowIfNull(_httpContextAccessor);

        var userIdClaim = _httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var id))
        {
            return new CurrentUserResponse(Guid.Empty, [], []);
        }

        var roles = GetClaimValues(ClaimTypes.Role);
        var permissions = GetClaimValues(CustomClaimTypes.Permission);

        return new CurrentUserResponse(
            id,
            roles,
            permissions,
            ManagedUserIds: [],
            ScopeKeys: []);
    }

    private List<string> GetClaimValues(string claimType) =>
        [.. _httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value).Distinct()];

    private string GetSingleClaimValue(string claimType) =>
        _httpContextAccessor.HttpContext!.User.Claims
            .Single(claim => claim.Type == claimType)
            .Value;
}
