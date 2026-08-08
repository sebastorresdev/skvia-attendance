using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.UploadAttendancePhoto;

public sealed class UploadAttendancePhotoCommandHandler(
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UploadAttendancePhotoCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> HandleAsync(
        UploadAttendancePhotoCommand command,
        CancellationToken cancellationToken)
    {
        var fileExtension = Path.GetExtension(command.FileName).ToLowerInvariant();

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        if (!allowedExtensions.Contains(fileExtension))
        {
            return Error.Validation("Upload.InvalidExtension", "El archivo debe ser una imagen (jpg, jpeg, png).");
        }

        var uploadDirectory = Path.Combine(environment.WebRootPath, "uploads", "attendances");
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var fullPath = Path.Combine(uploadDirectory, uniqueFileName);

        Directory.CreateDirectory(uploadDirectory);

        await using (command.FileStream)
        {
            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            await command.FileStream.CopyToAsync(fileStream, cancellationToken);
        }

        var request = httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var url = $"{baseUrl}/uploads/attendances/{uniqueFileName}";

        return url;
    }
}
