using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Application.Auth.Commands.Login;

public class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : ICommandHandler<LoginCommand, ErrorOr<ClaimsPrincipal>>
{
    public async Task<ErrorOr<ClaimsPrincipal>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.UserName);

        if (user is null)
        {
            return Error.Unauthorized("Credenciales Invalidas.");
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            return Error.Unauthorized("Usuario Bloqueado.");
        }

        if (!user.IsActive)
        {
            return Error.Unauthorized("Tu cuenta está inactiva. Ponte en contacto con el servicio de asistencia para obtener ayuda.");
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, command.Password);

        if (!isPasswordValid)
        {
            // Login Failed: Increment the failed access count
            await userManager.AccessFailedAsync(user);

            // Check if the account has been locked after this failure
            if (await userManager.IsLockedOutAsync(user))
            {
                return Error.Unauthorized("La cuenta ha sido bloqueada debido a múltiples intentos fallidos de inicio de sesión.");
            }
            else
            {
                return Error.Unauthorized("El nombre de usuario o la contraseña son incorrectos. Inténtalo de nuevo.");
            }

        }

        return await signInManager.CreateUserPrincipalAsync(user);
    }
}
