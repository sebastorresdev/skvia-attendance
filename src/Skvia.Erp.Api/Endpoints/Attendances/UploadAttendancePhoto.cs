using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Attendances.Commands.UploadAttendancePhoto;

namespace Skvia.Erp.Api.Endpoints.Attendances;

public sealed class UploadAttendancePhoto : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/mobile/upload-photo", Handle)
            .WithName(nameof(UploadAttendancePhoto))
            .WithSummary("Subir foto de asistencia")
            .WithDescription("Sube una foto desde la app móvil para ser adjuntada en la marcación.")
            .DisableAntiforgery() // Necesario para subida de archivos multipart sin token en Minimal APIs
            .Produces<UploadAttendancePhotoResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromForm] IFormFile file,
        ICommandHandler<UploadAttendancePhotoCommand, ErrorOr<string>> handler,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return Results.BadRequest(new ApiProblemDetails 
            { 
                Title = "Validation", 
                Detail = "No file uploaded." 
            });
        }

        await using var stream = file.OpenReadStream();
        var command = new UploadAttendancePhotoCommand(file.FileName, stream);
        
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            url => TypedResults.Ok(new UploadAttendancePhotoResponse(url)),
            errors => errors.ToProblem());
    }
}

public record UploadAttendancePhotoResponse(string PhotoUrl);



