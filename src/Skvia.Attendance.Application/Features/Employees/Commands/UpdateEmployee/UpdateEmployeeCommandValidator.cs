namespace Skvia.Attendance.Application.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id es obligatorio.");
        
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código del empleado es obligatorio.")
            .MaximumLength(20).WithMessage("El código del empleado no puede superar los 20 caracteres.")
            .Matches(@"^[a-zA-Z0-9_\-]+$").WithMessage("El código solo puede contener letras, números, guiones o guiones bajos.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido no puede superar los 100 caracteres.");

        RuleFor(x => x.DocumentType)
            .IsInEnum().WithMessage("El tipo de documento de identidad no es válido.");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("El número de documento es obligatorio.")
            .MaximumLength(30).WithMessage("El número de documento no puede superar los 30 caracteres.")
            .Matches(@"^[0-9A-Za-z\-]+$").WithMessage("El número de documento contiene caracteres no válidos.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.")
            .MaximumLength(150).WithMessage("El correo electrónico no puede superar los 150 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Email)); // Solo valida si se envía

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("El teléfono no puede superar los 20 caracteres.")
            .Matches(@"^\+?[0-9\s\-]+$").WithMessage("El formato del teléfono no es válido.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Position)
            .MaximumLength(100).WithMessage("El cargo no puede superar los 100 caracteres.");

        RuleFor(x => x.Department)
            .MaximumLength(100).WithMessage("El departamento/área no puede superar los 100 caracteres.");

        RuleFor(x => x.PhotoUrl)
            .MaximumLength(500).WithMessage("La URL de la foto no puede superar los 500 caracteres.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("La URL de la foto debe ser una dirección absoluta válida (ej: https://...).")
            .When(x => !string.IsNullOrEmpty(x.PhotoUrl));
        
    }
}
