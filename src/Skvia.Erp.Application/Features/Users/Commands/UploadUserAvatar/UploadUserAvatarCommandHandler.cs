using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Skvia.Erp.Application.Features.Users.Commands.UploadUserAvatar;

public sealed class UploadUserAvatarCommandHandler(
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UploadUserAvatarCommand, ErrorOr<FileUploadResponse>>
{
    public async Task<ErrorOr<FileUploadResponse>> HandleAsync(
        UploadUserAvatarCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Extensión del archivo
        var fileExtension = Path.GetExtension(command.FileName).ToLowerInvariant();

        // 2. Carpeta dentro de wwwroot
        var uploadDirectory = Path.Combine(environment.WebRootPath, "uploads", "users");

        // 3. Nombre único
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var fullPath = Path.Combine(uploadDirectory, uniqueFileName);

        Directory.CreateDirectory(uploadDirectory);

        // 4. Guardar archivo físico
        await using (command.FileStream)
        {
            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            await command.FileStream.CopyToAsync(fileStream, cancellationToken);
        }

        // 5. Construir URL pública completa
        var request = httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var url = $"{baseUrl}/uploads/users/{uniqueFileName}";

        return new FileUploadResponse(url);
    }
}


