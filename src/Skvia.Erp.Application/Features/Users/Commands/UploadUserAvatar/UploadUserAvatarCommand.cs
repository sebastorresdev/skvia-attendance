using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.UploadUserAvatar;

/// <summary>
/// Comando para subir o actualizar el avatar de un usuario.
/// </summary>
[AuthorizeCommand(Permissions = Permission.User.Update)]
public record UploadUserAvatarCommand(
    Stream FileStream,
    string FileName,
    long FileLength) : ICommand<ErrorOr<FileUploadResponse>>;



