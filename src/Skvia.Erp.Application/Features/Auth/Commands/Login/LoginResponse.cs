using System.Security.Claims;

namespace Skvia.Erp.Application.Features.Auth.Commands.Login;

public record LoginResponse(
    ClaimsPrincipal Principal);

