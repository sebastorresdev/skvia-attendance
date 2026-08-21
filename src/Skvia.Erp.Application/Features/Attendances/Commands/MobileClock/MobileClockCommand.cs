using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Common;

namespace Skvia.Erp.Application.Features.Attendances.Commands.MobileClock;

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


