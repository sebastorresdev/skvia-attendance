using FluentValidation;

namespace Skvia.Erp.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("El nombre de usuario no puede ser vacio.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("El correo es obligatorio.");

        RuleForEach(x => x.BranchIds)
            .Must(id => id != Guid.Empty)
            .WithMessage("BranchId inválido.");
    }
}

