namespace Skvia.Attendance.Application.Users.Commands.UploadUserAvatar;

public record UploadUserAvatarCommand(
    Stream FileStream,
    string FileName,
    long FileLength) : ICommand<ErrorOr<FileUploadResponse>>;
