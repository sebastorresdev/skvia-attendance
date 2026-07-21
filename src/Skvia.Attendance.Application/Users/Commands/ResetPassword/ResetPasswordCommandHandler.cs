using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Infrastructure.Identity.Domain;

namespace Skvia.Attendance.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler(UserManager<ApplicationUser> _userManager) : ICommandHandler<ResetPasswordCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        if (command.NewPassword != command.ConfirmNewPassword)
        {
            return Error.Conflict("Las contraseñas no coinciden.");
        }

        var removeResult = await _userManager.RemovePasswordAsync(user);

        if (!removeResult.Succeeded)
        {
            var errors = string.Join("; ", removeResult.Errors.Select(e => e.Description));
            return Error.Conflict($"No se pudo eliminar la contraseña actual: {errors}");
        }

        // Asigna la nueva contraseña
        var addResult = await _userManager.AddPasswordAsync(user, command.NewPassword);

        if (!addResult.Succeeded)
        {
            var errors = string.Join("; ", addResult.Errors.Select(e => e.Description));
            return Error.Conflict($"No se pudo asignar la nueva contraseña: {errors}");
        }

        return Result.Success;
    }
}
