using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using System.Security.Claims;

namespace Skvia.Erp.Application.Features.Auth.Commands.Login;

public record LoginCommand(string UserName, string Password) : ICommand<ErrorOr<ClaimsPrincipal>>;


