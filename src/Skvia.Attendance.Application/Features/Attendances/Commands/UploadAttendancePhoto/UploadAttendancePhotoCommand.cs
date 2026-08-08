using Skvia.Attendance.Application.Common.Interfaces;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.UploadAttendancePhoto;

public record UploadAttendancePhotoCommand(string FileName, Stream FileStream) : ICommand<ErrorOr<string>>;
