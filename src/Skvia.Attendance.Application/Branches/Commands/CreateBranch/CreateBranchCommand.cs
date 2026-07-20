namespace Skvia.Attendance.Application.Branches.Commands.CreateBranch;

public record CreateBranchCommand(string Code, string Name, string? Address) : ICommand<ErrorOr<Guid>>;
