using Skvia.Attendance.Application.Features.Auth.DTOs;

namespace Skvia.Attendance.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery() : IQuery<ErrorOr<CurrentUserResponse>>;
