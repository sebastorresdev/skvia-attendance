using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Common.Constants;
using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Application.Features.Users.Commands.SetUserPermissionOverrides;

public class SetUserPermissionOverridesCommandHandler(
    UserManager<ApplicationUser> userManager)
    : ICommandHandler<SetUserPermissionOverridesCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(
        SetUserPermissionOverridesCommand command, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return Error.NotFound("User.NotFound", "Usuario no encontrado");

        // 1. Quitar todos los overrides actuales (claims directos del usuario)
        var currentClaims = await userManager.GetClaimsAsync(user);
        var currentPermissionClaims = currentClaims
            .Where(c => c.Type == CustomClaimTypes.Permission)
            .ToList();

        if (currentPermissionClaims.Count > 0)
        {
            var removeResult = await userManager.RemoveClaimsAsync(user, currentPermissionClaims);
            if (!removeResult.Succeeded)
                return Error.Failure("Permissions.RemoveFailed", "No se pudieron limpiar los permisos actuales");
        }

        // 2. Agregar los nuevos overrides seleccionados
        var newClaims = command.PermissionKeys
            .Select(key => new Claim(CustomClaimTypes.Permission, key))
            .ToList();

        if (newClaims.Count > 0)
        {
            var addResult = await userManager.AddClaimsAsync(user, newClaims);
            if (!addResult.Succeeded)
                return Error.Failure("Permissions.AddFailed", "No se pudieron asignar los nuevos permisos");
        }

        return Result.Success;
    }
}
