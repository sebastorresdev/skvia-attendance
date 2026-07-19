using Microsoft.AspNetCore.Http;

using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Common.Models;

using System.Security.Claims;

namespace Skvia.Attendance.Infrastructure.Security.CurrentUserProvider;

public class CurrentUserProvider(IHttpContextAccessor _httpContextAccessor) : ICurrentUserProvider
{
    public CurrentUser GetCurrentUser()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            return new CurrentUser(Guid.Empty, [], []);
        }

        ArgumentNullException.ThrowIfNull(_httpContextAccessor);

        var id = Guid.Parse(GetSingleClaimValue("id"));
        var permissions = GetClaimValues("permissions");
        var roles = GetClaimValues(ClaimTypes.Role);

        return new CurrentUser(id, permissions, roles);
    }

    private List<string> GetClaimValues(string claimType) =>
        [.. _httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)];

    private string GetSingleClaimValue(string claimType) =>
        _httpContextAccessor.HttpContext!.User.Claims
            .Single(claim => claim.Type == claimType)
            .Value;
}
