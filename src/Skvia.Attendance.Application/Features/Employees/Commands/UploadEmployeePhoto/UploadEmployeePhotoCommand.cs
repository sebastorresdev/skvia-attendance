namespace Skvia.Attendance.Application.Features.Employees.Commands.UploadEmployeePhoto;

public record UploadEmployeePhotoCommand(
    Stream FileStream,
    string FileName,
    long FileLength) : ICommand<ErrorOr<UploadEmployeePhotoResponse>>;
    
public record UploadEmployeePhotoResponse(string Url);
