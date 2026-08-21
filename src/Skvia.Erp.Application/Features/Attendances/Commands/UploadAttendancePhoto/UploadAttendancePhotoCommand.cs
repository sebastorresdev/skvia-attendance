using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;

namespace Skvia.Erp.Application.Features.Attendances.Commands.UploadAttendancePhoto;

public record UploadAttendancePhotoCommand(string FileName, Stream FileStream) : ICommand<ErrorOr<string>>;


