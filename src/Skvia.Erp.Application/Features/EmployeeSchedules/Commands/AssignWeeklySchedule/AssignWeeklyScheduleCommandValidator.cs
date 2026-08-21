using FluentValidation;
namespace Skvia.Erp.Application.Features.EmployeeSchedules.Commands.AssignWeeklySchedule;

public class AssignWeeklyScheduleCommandValidator : AbstractValidator<AssignWeeklyScheduleCommand>
{
    public AssignWeeklyScheduleCommandValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("La fecha de fin debe ser mayor o igual a la de inicio.");
        
        RuleFor(x => x.Days).NotEmpty().WithMessage("Debe asignar al menos un día.");
        
        RuleForEach(x => x.Days).ChildRules(days =>
        {
            days.RuleFor(d => d.Date).NotEmpty();
            days.RuleFor(d => d.DayType).IsInEnum();
        });
    }
}


