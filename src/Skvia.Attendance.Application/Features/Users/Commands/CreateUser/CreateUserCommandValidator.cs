namespace Skvia.Attendance.Application.Features.Users.Commands.CreateUser;

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

        RuleFor(x => x.BranchIds)
        .NotNull().WithMessage("BranchIds es requerido.")
        .NotEmpty().WithMessage("Debe seleccionar al menos una sucursal.");

        RuleFor(x => x.RoleIds)
        .NotNull().WithMessage("RoleIds es requerido.");
    }
}

