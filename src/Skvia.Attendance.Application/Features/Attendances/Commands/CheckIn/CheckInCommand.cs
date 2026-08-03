using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.CheckIn;

public record CheckInCommand(
    string EmployeeIdentifier, // Can be DNI or Code
    Guid BranchId,
    string PhotoUrl) : ICommand<ErrorOr<Success>>;
