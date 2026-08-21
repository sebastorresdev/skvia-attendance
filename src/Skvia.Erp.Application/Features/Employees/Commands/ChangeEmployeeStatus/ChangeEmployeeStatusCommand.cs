using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Common;
using Skvia.Erp.Domain.Employees;
using ErrorOr;

namespace Skvia.Erp.Application.Features.Employees.Commands.ChangeEmployeeStatus;

public record ChangeEmployeeStatusCommand(
    Guid EmployeeId,
    EmployeeStatus NewStatus) : ICommand<ErrorOr<Success>>;


