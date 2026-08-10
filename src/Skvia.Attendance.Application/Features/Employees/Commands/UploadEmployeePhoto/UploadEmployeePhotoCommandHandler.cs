using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Skvia.Attendance.Application.Common.Interfaces;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Employees.Commands.UploadEmployeePhoto;

public sealed class UploadEmployeePhotoCommandHandler(
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UploadEmployeePhotoCommand, ErrorOr<UploadEmployeePhotoResponse>>
{
    public async Task<ErrorOr<UploadEmployeePhotoResponse>> HandleAsync(
        UploadEmployeePhotoCommand command,
        CancellationToken cancellationToken)
    {
        // 1. File extension
        var fileExtension = Path.GetExtension(command.FileName).ToLowerInvariant();

        // 2. Folder inside wwwroot
        var uploadDirectory = Path.Combine(environment.WebRootPath, "uploads", "employees");

        // 3. Unique filename
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var fullPath = Path.Combine(uploadDirectory, uniqueFileName);

        Directory.CreateDirectory(uploadDirectory);

        // 4. Save file physically
        await using (command.FileStream)
        {
            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            await command.FileStream.CopyToAsync(fileStream, cancellationToken);
        }

        // 5. Build public URL
        var request = httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var url = $"{baseUrl}/uploads/employees/{uniqueFileName}";

        return new UploadEmployeePhotoResponse(url);
    }
}
