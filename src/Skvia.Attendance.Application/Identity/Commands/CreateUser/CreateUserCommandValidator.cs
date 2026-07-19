using FluentValidation;

namespace Skvia.Attendance.Application.Identity.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("El nombre de usuario es requerido.");

        RuleForEach(x => x.BranchIds)
            .Must(id => id != Guid.Empty)
            .WithMessage("BranchId inválido.");

        RuleForEach(x => x.RoleIds)
            .Must(id => id != Guid.Empty)
            .WithMessage("RoleId inválido.");
    }
}

