namespace Skvia.Attendance.Application.Features.Branches.Commands.UnarchiveBranch;

public record UnarchiveBranchCommand(Guid BranchId) : ICommand<ErrorOr<Success>>;
