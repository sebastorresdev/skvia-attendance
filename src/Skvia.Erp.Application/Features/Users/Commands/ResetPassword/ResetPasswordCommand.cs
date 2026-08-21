using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.ResetPassword;

/// <summary>
/// Comando para restablecer la contraseña de un usuario.
/// </summary>
[AuthorizeCommand(Permissions = Permission.User.Update)]
public record ResetPasswordCommand(string UserId, string NewPassword, string ConfirmNewPassword) : ICommand<ErrorOr<Success>>;



