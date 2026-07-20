using System.Security.Claims;

namespace Skvia.Attendance.Application.Auth.Commands.Login;

public record LoginCommand(string UserName, string Password) : ICommand<ErrorOr<ClaimsPrincipal>>;
