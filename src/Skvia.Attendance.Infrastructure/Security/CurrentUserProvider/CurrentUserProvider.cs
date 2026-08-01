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
        if (_httpContextAccessor.HttpContext == null)
        {
            return new CurrentUserResponse(Guid.Empty, [], []);
        }

        ArgumentNullException.ThrowIfNull(_httpContextAccessor);

        var id = Guid.Parse(GetSingleClaimValue(ClaimTypes.NameIdentifier));
        var roles = GetClaimValues(ClaimTypes.Role);
        var permissions = GetClaimValues(CustomClaimTypes.Permission);

        return new CurrentUserResponse(id, roles, permissions);
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
