using Skvia.Attendance.Application.Common.Models;

namespace Skvia.Attendance.Application.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery() : IQuery<ErrorOr<CurrentUser>>;
