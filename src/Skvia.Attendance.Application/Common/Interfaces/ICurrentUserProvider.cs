using Skvia.Attendance.Application.Features.Auth.DTOs;

namespace Skvia.Attendance.Application.Common.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUserResponse GetCurrentUser();
}
