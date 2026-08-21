using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Users.Commands.UploadUserAvatar;

namespace Skvia.Erp.Api.Endpoints.Users;

public sealed class UploadUserAvatar : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/avatar", Handle)
            .WithName(nameof(UploadUserAvatar))
            .WithSummary("Subir foto de usuario")
            .WithDescription("Sube la foto de perfil del usuario y retorna los detalles del archivo cargado.")
            .Produces<FileUploadResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict)
            .DisableAntiforgery()
            .WithRequestTimeout("UploadPolicy");

    private static async Task<IResult> Handle(
        [FromForm] IFormFile? avatar,
        ICommandHandler<UploadUserAvatarCommand, ErrorOr<FileUploadResponse>> handler,
        CancellationToken cancellationToken)
    {
        if (avatar is null || avatar.Length == 0)
        {
            var error = Error.Validation(
                code: "Avatar.Empty",
                description: "El archivo enviado no puede estar vacío.");

            return new[] { error }.ToProblem();
        }

        using var fileStream = avatar.OpenReadStream();

        var command = new UploadUserAvatarCommand(
            FileStream: fileStream,
            FileName: avatar.FileName,
            FileLength: avatar.Length);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}



