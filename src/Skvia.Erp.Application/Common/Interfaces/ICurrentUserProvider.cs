using Skvia.Erp.Application.Features.Auth.DTOs;

namespace Skvia.Erp.Application.Common.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUserResponse GetCurrentUser();
}

