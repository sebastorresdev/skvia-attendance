using System.Security.Claims;

namespace Skvia.Attendance.Application.Features.Auth.Commands.Login;

public record LoginResponse(
    ClaimsPrincipal Principal);
