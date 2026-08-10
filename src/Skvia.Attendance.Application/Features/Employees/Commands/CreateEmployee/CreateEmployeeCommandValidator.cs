using FluentValidation;
using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código del empleado es obligatorio.")
            .MaximumLength(EmployeeConstants.CodeMaxLength).WithMessage($"El código del empleado no puede superar los {EmployeeConstants.CodeMaxLength} caracteres.")
            .Matches(@"^[a-zA-Z0-9_\-]+$").WithMessage("El código solo puede contener letras, números, guiones o guiones bajos.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(EmployeeConstants.FirstNameMaxLength).WithMessage($"El nombre no puede superar los {EmployeeConstants.FirstNameMaxLength} caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(EmployeeConstants.LastNameMaxLength).WithMessage($"El apellido no puede superar los {EmployeeConstants.LastNameMaxLength} caracteres.");

        RuleFor(x => x.DocumentType)
            .IsInEnum().WithMessage("El tipo de documento de identidad no es válido.");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("El número de documento es obligatorio.")
            .Matches(@"^[0-9A-Za-z\-]+$").WithMessage("El número de documento contiene caracteres no válidos.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[\d\s-]{7,20}$").WithMessage("El formato del teléfono no es válido.") // Using the regex from the Phone value object
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Position)
            .MaximumLength(EmployeeConstants.PositionMaxLength).WithMessage($"El cargo no puede superar los {EmployeeConstants.PositionMaxLength} caracteres.");


        RuleFor(x => x.PhotoUrl)
            .MaximumLength(EmployeeConstants.PhotoUrlMaxLength).WithMessage($"La URL de la foto no puede superar los {EmployeeConstants.PhotoUrlMaxLength} caracteres.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("La URL de la foto debe ser una dirección absoluta válida (ej: https://...).")
            .When(x => !string.IsNullOrEmpty(x.PhotoUrl));

        RuleFor(x => x.ApplicationUserId)
            .NotEmpty().WithMessage("Debe seleccionar un usuario del sistema para habilitar la marcación móvil.")
            .When(x => x.MobileCheckInEnabled);
            
        RuleFor(x => x.MobileCheckInEnabled)
            .Equal(false).WithMessage("No se puede habilitar la marcación móvil si no se controla la asistencia.")
            .When(x => !x.IsAttendanceTracked);
    }
}
