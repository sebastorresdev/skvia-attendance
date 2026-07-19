using Skvia.Attendance.Application.Users.DTOs;

namespace Skvia.Attendance.Application.Users.Queries.GetUsers;

public record GetUsersQuery() : IQuery<ErrorOr<List<UserResponse>>>;
