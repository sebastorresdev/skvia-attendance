namespace Skvia.Attendance.Application.Branches.Commands.UnarchiveBranch;

public record UnarchiveBranchCommand(Guid BranchId) : ICommand<ErrorOr<Success>>;
