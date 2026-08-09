using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.MobileClock;

public record MobileClockCommand(
    string ApplicationUserId,
    string UserName,
    string TipoMarcacion,
    double? Latitud,
    double? Longitud,
    string? PhotoUrl) : ICommand<ErrorOr<MobileClockResult>>;

public record MobileClockResult(
    bool Success,
    string Message,
    string Timestamp,
    string TipoMarcacion,
    double? Latitud,
    double? Longitud);
