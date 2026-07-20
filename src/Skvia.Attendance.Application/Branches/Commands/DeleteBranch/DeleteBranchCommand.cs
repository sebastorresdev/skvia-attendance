namespace Skvia.Attendance.Application.Branches.Commands.DeleteBranch;

public record DeleteBranchCommand(Guid BranchId) : ICommand<ErrorOr<Success>>;
