using FluentValidation;

using Skvia.Attendance.Domain.Branches;

namespace Skvia.Attendance.Application.Features.Branches.Commands.UpdateBranch;

public class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es requerido.")
            .MaximumLength(BranchConstants.CodeMaxLength)
            .WithMessage($"El código no puede superar los {BranchConstants.CodeMaxLength} caracteres.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(BranchConstants.NameMaxLength)
            .WithMessage($"El nombre no puede superar los {BranchConstants.NameMaxLength} caracteres.");
        RuleFor(x => x.Address)
            .MaximumLength(BranchConstants.AddressMaxLength)
            .WithMessage($"La dirección no puede superar los {BranchConstants.AddressMaxLength} caracteres.")
            .When(x => x.Address != null);
    }
}
