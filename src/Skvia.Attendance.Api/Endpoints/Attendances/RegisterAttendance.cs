using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Application.Features.Attendances.Commands.CheckIn;
using Skvia.Attendance.Application.Features.Attendances.Commands.CheckOut;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Domain.Attendances;

namespace Skvia.Attendance.Api.Endpoints.Attendances;

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
        Skvia.Attendance.Application.Common.Interfaces.IApplicationDbContext dbContext,
        ICommandHandler<CheckInCommand, ErrorOr<Success>> checkInHandler,
        ICommandHandler<Skvia.Attendance.Application.Features.Attendances.Commands.StartBreak.StartBreakCommand, ErrorOr<Success>> startBreakHandler,
        ICommandHandler<Skvia.Attendance.Application.Features.Attendances.Commands.EndBreak.EndBreakCommand, ErrorOr<Success>> endBreakHandler,
        ICommandHandler<CheckOutCommand, ErrorOr<Success>> checkOutHandler,
        CancellationToken cancellationToken)
    {
        var userName = user.Identity?.Name;
        if (string.IsNullOrEmpty(userName)) return TypedResults.Unauthorized();

        var employee = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            dbContext.Employees, e => e.ApplicationUserId == user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value || e.Code == userName, cancellationToken);
            
        if (employee is null || !employee.MainBranchId.HasValue)
            return TypedResults.Problem("Empleado no encontrado o sin sede principal asignada.", statusCode: 400);

        ErrorOr<Success> result;
        var photo = request.PhotoUrl ?? "mobile-default-photo.jpg"; // Default photo for testing

        switch (request.TipoMarcacion.ToUpper())
        {
            case "ENTRADA":
                result = await checkInHandler.HandleAsync(new CheckInCommand(employee.Code, employee.MainBranchId.Value, photo, AttendanceSource.Mobile, request.Latitud, request.Longitud), cancellationToken);
                break;
            case "INICIO_REFRIGERIO":
                result = await startBreakHandler.HandleAsync(new Skvia.Attendance.Application.Features.Attendances.Commands.StartBreak.StartBreakCommand(employee.Code, employee.MainBranchId.Value, photo, AttendanceSource.Mobile, request.Latitud, request.Longitud), cancellationToken);
                break;
            case "FIN_REFRIGERIO":
                result = await endBreakHandler.HandleAsync(new Skvia.Attendance.Application.Features.Attendances.Commands.EndBreak.EndBreakCommand(employee.Code, employee.MainBranchId.Value, photo, AttendanceSource.Mobile, request.Latitud, request.Longitud), cancellationToken);
                break;
            case "SALIDA":
                result = await checkOutHandler.HandleAsync(new CheckOutCommand(employee.Code, employee.MainBranchId.Value, photo, AttendanceSource.Mobile, request.Latitud, request.Longitud), cancellationToken);
                break;
            default:
                return TypedResults.Problem("Tipo de marcación inválido.", statusCode: 400);
        }

        return result.Match(
            _ => TypedResults.Ok(new MobileClockResponse(true, $"Marcación de {request.TipoMarcacion} registrada con éxito.", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), request.TipoMarcacion, request.Latitud, request.Longitud)),
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
            request.BranchId, 
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
            request.BranchId, 
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
    Guid BranchId, 
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
