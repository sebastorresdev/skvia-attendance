using Skvia.Attendance.Application.Features.Users.Commands.UploadUserAvatar;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class UploadUserAvatar : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/avatar", Handle)
            .WithSummary("Subir foto de usuario")
            .WithDescription("Sube la foto de perfil y retorna la URL usando el pipeline de comandos.")
            .DisableAntiforgery()
            .Produces<FileUploadResponse>();

    private static async Task<IResult> Handle(
        IFormFile? avatar, // .NET Minimal APIs mapea multipart/form-data automáticamente aquí
        ICommandHandler<UploadUserAvatarCommand, ErrorOr<FileUploadResponse>> handler,
        CancellationToken ct)
    {
        // Validación HTTP superficial básica antes de instanciar recursos
        if (avatar is null || avatar.Length == 0)
        {
            return TypedResults.BadRequest();
        }

        // Abrimos el stream del archivo directamente desde la petición HTTP
        var fileStream = avatar.OpenReadStream();

        // Construimos nuestro comando limpio de aplicación
        var command = new UploadUserAvatarCommand(
            FileStream: fileStream,
            FileName: avatar.FileName,
            FileLength: avatar.Length);

        var result = await handler.HandleAsync(command, ct);

        // Retornamos de manera consistente con el patrón Match de ErrorOr
        return result.Match(
            TypedResults.Ok,
            ResultExtensions.ToProblem);
    }
}
