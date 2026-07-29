using Skvia.Attendance.Application.Features.Users.DTOs;

namespace Skvia.Attendance.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery() : IQuery<ErrorOr<List<UserResponse>>>;
