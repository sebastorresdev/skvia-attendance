using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.CheckOut;

public record CheckOutCommand(
    string EmployeeIdentifier,
    Guid BranchId,
    string PhotoUrl) : ICommand<ErrorOr<Success>>;
