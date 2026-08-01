namespace Skvia.Attendance.Api.Endpoints.Users.Requests;

public record DeleteUsersBatchRequest(List<Guid> UserIds);
