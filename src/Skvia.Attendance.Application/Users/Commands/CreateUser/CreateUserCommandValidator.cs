using FluentValidation;

namespace Skvia.Attendance.Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("El nombre de usuario es requerido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es obligatorio.");

        RuleForEach(x => x.BranchIds)
            .Must(id => id != Guid.Empty)
            .WithMessage("BranchId inválido.");
    }
}

