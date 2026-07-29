namespace Skvia.Attendance.Application.Features.Branches.Commands.DeleteBranch;

public record DeleteBranchCommand(Guid BranchId) : ICommand<ErrorOr<Success>>;
