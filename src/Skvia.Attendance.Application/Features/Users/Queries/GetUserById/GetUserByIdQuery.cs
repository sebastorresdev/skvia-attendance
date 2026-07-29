using Skvia.Attendance.Application.Features.Users.DTOs;

namespace Skvia.Attendance.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IQuery<ErrorOr<UserDetailResponse>>;
