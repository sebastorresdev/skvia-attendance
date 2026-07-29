using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Common.Constants;
using Skvia.Attendance.Application.Common.Security.Permissions;
using Skvia.Attendance.Application.Features.Users.DTOs;
using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Application.Features.Users.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
    : IQueryHandler<GetUserPermissionsQuery, ErrorOr<List<PermissionGroupDto>>>
{
    public async Task<ErrorOr<List<PermissionGroupDto>>> HandleAsync(
        GetUserPermissionsQuery query, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return Error.NotFound("User.NotFound", "Usuario no encontrado");

        // 1. Permisos heredados de TODOS los roles del usuario
        var roleNames = await userManager.GetRolesAsync(user);
        var rolePermissionKeys = new HashSet<string>();

        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null) continue;

            var roleClaims = await roleManager.GetClaimsAsync(role);
            foreach (var claim in roleClaims.Where(c => c.Type == CustomClaimTypes.Permission))
                rolePermissionKeys.Add(claim.Value);
        }

        // 2. Permisos asignados directamente al usuario (overrides)
        var userClaims = await userManager.GetClaimsAsync(user);
        var overrideKeys = userClaims
            .Where(c => c.Type == CustomClaimTypes.Permission)
            .Select(c => c.Value)
            .ToHashSet();

        // 3. Catálogo completo del sistema
        var catalog = PermissionCatalog.GetAll(); // el mismo que ya usas en el drawer de Roles

        var result = catalog.Select(g => new PermissionGroupDto(
            g.Group,
            g.GroupDescription,
            g.Permissions.Select(p =>
            {
                var fromRole = rolePermissionKeys.Contains(p.Key);
                var fromOverride = overrideKeys.Contains(p.Key);

                return new PermissionItemDto(
                    p.Key,
                    p.Display,
                    p.Description,
                    Granted: fromRole || fromOverride,
                    Source: fromRole ? "Role" : fromOverride ? "Override" : null
                );
            }).ToList()
        )).ToList();

        return result;
    }
}
