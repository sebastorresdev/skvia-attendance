using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Common.Constants;
using Skvia.Attendance.Application.Common.DTOs;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Common.Security.Permissions;
using Skvia.Attendance.Application.Common.Security.Roles;
using Skvia.Attendance.Application.Features.Roles.Commands.CreateRole;
using Skvia.Attendance.Application.Features.Roles.Commands.DeleteRole;
using Skvia.Attendance.Application.Features.Roles.Commands.UpdateRole;
using Skvia.Attendance.Application.Features.Roles.Commands.SetRolePermissions;
using Skvia.Attendance.Application.Features.Roles.DTOs;
using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Infrastructure.Services;

internal class IdentityRoleService(
    RoleManager<ApplicationRole> roleManager) : IRoleService
{
    public async Task<ErrorOr<Guid>> CreateRoleAsync(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = new ApplicationRole
        {
            Name = command.Name,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        };

        var identityResult = await roleManager.CreateAsync(role);

        if (!identityResult.Succeeded)
        {
            return identityResult.ToApplicationError();
        }

        return role.Id;
    }

    public async Task<ErrorOr<Success>> DeleteRoleAsync(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var roleAdmin = await roleManager.FindByNameAsync(Roles.Administrator) ?? throw new ApplicationException("No se pudo encontrar el rol de administrador.");

        if (command.RoleIds.Contains(roleAdmin.Id))
        {
            return Error.Validation("Role.Delete.Administrator", "No se puede eliminar el rol de Administrador.");
        }

        var affectedRows = await roleManager.Roles
            .Where(r => command.RoleIds.Contains(r.Id))
            .ExecuteDeleteAsync(cancellationToken);

        if (affectedRows <= 0)
        {
            return Error.Conflict("No se pudo eliminar los roles");
        }

        return Result.Success;

    }

    public async Task<ErrorOr<RoleResponse>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());

        return role is null
            ? Error.NotFound("Role.NotFound", $"Rol con Id '{roleId}' no encontrado.")
            : new RoleResponse(role.Id, role.Name!, role.Description);
    }

    public async Task<ErrorOr<List<PermissionGroupResponse>>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());

        if (role is null)
        {
            return Error.NotFound("Role.NotFound", $"Rol con Id '{roleId}' no encontrado.");
        }

        var rolePermissionKeys = new HashSet<string>();
        var roleClaims = await roleManager.GetClaimsAsync(role);

        foreach (var claim in roleClaims.Where(c => c.Type == CustomClaimTypes.Permission))
        {
            rolePermissionKeys.Add(claim.Value);
        }

        var catalog = PermissionCatalog.GetAll();
        var result = catalog.Select(g => new PermissionGroupResponse(
            g.Group,
            g.GroupDescription,
            g.Permissions.Select(p =>
            {
                var fromRole = rolePermissionKeys.Contains(p.Key);

                return new PermissionItemResponse(
                    p.Key,
                    p.Display,
                    p.Description,
                    Granted: fromRole,
                    Source: fromRole ? "Override" : null);
            }).ToList()
        )).ToList();

        return result;
    }

    public async Task<ErrorOr<List<RoleResponse>>> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);

        return roles.Select(r => new RoleResponse(r.Id, r.Name!, r.Description)).ToList();
    }

    public async Task<ErrorOr<Success>> UpdateRoleAsync(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var existingRole = await roleManager.FindByIdAsync(command.Id.ToString());

        if (existingRole is null)
        {
            return Error.NotFound("Role.NotFound", $"Rol con Id '{command.Id}' no encontrado.");
        }

        existingRole.Name = command.Name;
        existingRole.Description = command.Description;
        existingRole.LastModifiedAt = DateTime.UtcNow;

        var result = await roleManager.UpdateAsync(existingRole);
        if (!result.Succeeded)
        {
            return result.ToApplicationError();
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> SetRolePermissionsAsync(SetRolePermissionsCommand command, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(command.RoleId.ToString());
        if (role is null)
        {
            return Error.NotFound("Role.NotFound", "Rol no encontrado");
        }

        var currentClaims = await roleManager.GetClaimsAsync(role);
        var currentPermissionClaims = currentClaims.Where(c => c.Type == CustomClaimTypes.Permission).ToList();

        // Remove old claims
        foreach (var claim in currentPermissionClaims)
        {
            var removeResult = await roleManager.RemoveClaimAsync(role, claim);
            if (!removeResult.Succeeded)
            {
                return Error.Failure("Permissions.RemoveFailed", "No se pudieron limpiar los permisos actuales del rol");
            }
        }

        // Add new claims
        foreach (var key in command.PermissionKeys)
        {
            var addResult = await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(CustomClaimTypes.Permission, key));
            if (!addResult.Succeeded)
            {
                return Error.Failure("Permissions.AddFailed", "No se pudieron asignar los nuevos permisos al rol");
            }
        }

        return Result.Success;
    }
}
