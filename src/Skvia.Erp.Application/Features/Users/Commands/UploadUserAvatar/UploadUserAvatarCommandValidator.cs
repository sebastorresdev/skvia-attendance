using FluentValidation;
namespace Skvia.Erp.Application.Features.Users.Commands.UploadUserAvatar;

public sealed class UploadUserAvatarCommandValidator : AbstractValidator<UploadUserAvatarCommand>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public UploadUserAvatarCommandValidator()
    {
        // 🌟 Validación de la Extensión
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("El nombre del archivo no puede estar vacío.")
            .Must(HaveAllowedExtension)
            .WithMessage($"Solo se permiten imágenes con las siguientes extensiones: {string.Join(", ", AllowedExtensions)}");

        // 🌟 Validación del Tamaño (Max 2MB)
        RuleFor(x => x.FileLength)
            .GreaterThan(0).WithMessage("El archivo está vacío.")
            .LessThanOrEqualTo(2 * 1024 * 1024).WithMessage("La imagen no puede superar los 2MB.");
    }

    private bool HaveAllowedExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}


