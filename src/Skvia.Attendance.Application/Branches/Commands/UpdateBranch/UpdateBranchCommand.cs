namespace Skvia.Attendance.Application.Branches.Commands.UpdateBranch;

public record UpdateBranchCommand(Guid BranchId, string Code, string Name, string? Address) : ICommand<ErrorOr<Success>>;
