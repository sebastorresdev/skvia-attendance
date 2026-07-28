using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Infrastructure.Services;

public class UserPermissionService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : IUserPermissionService
{
    public async Task<List<string>> GetPermissionsAsync(ApplicationUser user)
    {
        List<string> directPermissions = await GetUserPermissionsAsync(user);
        List<string> inheritedPermissions = await GetInheritedUserPermissionsAsync(user);

        var permissions = directPermissions
            .Union(inheritedPermissions)
            .Distinct()
            .ToList();

        return permissions;
    }

    private async Task<List<string>> GetUserPermissionsAsync(ApplicationUser user)
    {
        var claims = await userManager.GetClaimsAsync(user);

        return claims.Where(c => c.Type.Equals("permissions", StringComparison.OrdinalIgnoreCase)).Select(p => p.Value).ToList();
    }

    private async Task<List<string>> GetInheritedUserPermissionsAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        if (!roles.Any())
        {
            return [];
        }

        var assignedRoles = await roleManager.Roles
            .Where(role => role.Name != null && roles.Contains(role.Name))
            .ToListAsync();

        var inheritedClaims = new List<Claim>();

        foreach (var role in assignedRoles)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            inheritedClaims.AddRange(claims);
        }

        return inheritedClaims
            .Where(c => c.Type.Equals("permissions", StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Value).Distinct().ToList();
    }
}
