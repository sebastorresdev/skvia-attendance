using System.Security.Claims;

namespace Skvia.Attendance.Application.Auth.Commands.Login;

public record LoginResponse(
    ClaimsPrincipal Principal);
