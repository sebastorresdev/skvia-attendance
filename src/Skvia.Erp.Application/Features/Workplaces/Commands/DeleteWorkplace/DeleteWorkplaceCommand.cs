using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Security;

namespace Skvia.Erp.Application.Features.Workplaces.Commands.DeleteWorkplace;

[AuthorizeCommand(Permissions = Permission.Workplace.Delete)]
public record DeleteWorkplaceCommand(Guid Id) : ICommand<ErrorOr<Success>>;


