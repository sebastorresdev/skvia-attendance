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
        var command = new CheckOutCommand(
            request.EmployeeIdentifier, 
            request.BranchId, 
            request.PhotoUrl);

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
