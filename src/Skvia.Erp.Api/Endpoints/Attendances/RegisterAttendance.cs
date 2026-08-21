using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Application.Features.Attendances.Commands.CheckIn;
using Skvia.Erp.Application.Features.Attendances.Commands.CheckOut;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Domain.Attendances;

namespace Skvia.Erp.Api.Endpoints.Attendances;

public sealed class RegisterAttendance : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/check-in", HandleCheckIn)
            .WithName(nameof(HandleCheckIn))
            .WithSummary("Registrar Entrada")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .AllowAnonymous();

        group.MapPost("/check-out", HandleCheckOut)
            .WithName(nameof(HandleCheckOut))
            .WithSummary("Registrar Salida")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .AllowAnonymous();
            
        group.MapPost("/mobile/clock", HandleMobileClock)
            .WithName(nameof(HandleMobileClock))
            .WithSummary("Marcación Móvil Unificada")
            .Produces<MobileClockResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleMobileClock(
        [FromBody] MobileClockRequest request,
        System.Security.Claims.ClaimsPrincipal user,
        ICommandHandler<Application.Features.Attendances.Commands.MobileClock.MobileClockCommand, ErrorOr<Application.Features.Attendances.Commands.MobileClock.MobileClockResult>> mobileClockHandler,
        CancellationToken cancellationToken)
    {
        var userName = user.Identity?.Name;
        if (string.IsNullOrEmpty(userName)) return TypedResults.Unauthorized();

        var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        var command = new Application.Features.Attendances.Commands.MobileClock.MobileClockCommand(
            ApplicationUserId: userId ?? string.Empty,
            UserName: userName,
            TipoMarcacion: request.TipoMarcacion,
            Latitud: request.Latitud,
            Longitud: request.Longitud,
            PhotoUrl: request.PhotoUrl
        );

        var result = await mobileClockHandler.HandleAsync(command, cancellationToken);

        return result.Match(
            success => TypedResults.Ok(success),
            errors => errors.ToProblem());
    }

    private static async Task<IResult> HandleCheckIn(
        [FromBody] AttendanceRequest request,
        ICommandHandler<CheckInCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var source = request.Source ?? AttendanceSource.Kiosk;

        var command = new CheckInCommand(
            request.EmployeeIdentifier, 
            request.WorkplaceId, 
            request.PhotoUrl,
            source,
            request.Latitude,
            request.Longitude,
            request.DeviceName,
            request.DeviceToken);

        var result = await handler.HandleAsync(command, cancellationToken);
        
        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }

    private static async Task<IResult> HandleCheckOut(
        [FromBody] AttendanceRequest request,
        ICommandHandler<CheckOutCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var source = request.Source ?? AttendanceSource.Kiosk;

        var command = new CheckOutCommand(
            request.EmployeeIdentifier, 
            request.WorkplaceId, 
            request.PhotoUrl,
            source,
            request.Latitude,
            request.Longitude);

        var result = await handler.HandleAsync(command, cancellationToken);
        
        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public record AttendanceRequest(
    string EmployeeIdentifier, 
    Guid WorkplaceId, 
    string PhotoUrl,
    AttendanceSource? Source,
    double? Latitude,
    double? Longitude,
    string? DeviceToken,
    string? DeviceName);

public record MobileClockRequest(
    string TipoMarcacion,
    double? Latitud,
    double? Longitud,
    string? PhotoUrl);

public record MobileClockResponse(
    bool Success,
    string Message,
    string Timestamp,
    string TipoMarcacion,
    double? Latitud,
    double? Longitud);



