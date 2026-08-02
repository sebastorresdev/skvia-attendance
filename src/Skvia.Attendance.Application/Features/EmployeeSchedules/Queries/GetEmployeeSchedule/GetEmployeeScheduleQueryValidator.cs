using FluentValidation;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Queries.GetEmployeeSchedule;

public class GetEmployeeScheduleQueryValidator : AbstractValidator<GetEmployeeScheduleQuery>
{
    public GetEmployeeScheduleQueryValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("La fecha de fin debe ser mayor o igual a la de inicio.");
    }
}
