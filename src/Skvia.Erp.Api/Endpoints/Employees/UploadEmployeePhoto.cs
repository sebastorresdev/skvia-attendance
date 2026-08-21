using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
using Skvia.Erp.Application.Features.Employees.Commands.UploadEmployeePhoto;

namespace Skvia.Erp.Api.Endpoints.Employees;

public sealed class UploadEmployeePhoto : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/upload-photo", Handle)
            .WithName(nameof(UploadEmployeePhoto))
            .DisableAntiforgery()
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<UploadEmployeePhotoResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithRequestTimeout("UploadPolicy");

    public static async Task<IResult> Handle(
        [FromForm] IFormFile? file,
        ICommandHandler<UploadEmployeePhotoCommand, ErrorOr<UploadEmployeePhotoResponse>> handler,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            var error = Error.Validation(
                code: "Photo.Empty",
                description: "El archivo enviado no puede estar vacío.");

            return new[] { error }.ToProblem();
        }

        using var stream = file.OpenReadStream();
        var command = new UploadEmployeePhotoCommand(stream, file.FileName, file.Length);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}



