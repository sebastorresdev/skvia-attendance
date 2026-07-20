namespace Skvia.Attendance.Application.Branches.Commands.ArchiveBranch;

public record ArchiveBranchCommand(Guid BranchId) : ICommand<ErrorOr<Success>>;
