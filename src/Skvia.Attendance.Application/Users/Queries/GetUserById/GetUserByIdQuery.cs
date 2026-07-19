using Skvia.Attendance.Application.Users.DTOs;

namespace Skvia.Attendance.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IQuery<ErrorOr<UserDetailResponse>>;
