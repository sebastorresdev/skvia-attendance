namespace Skvia.Attendance.Application.Features.Branches.Commands.CreateBranch;

public record CreateBranchCommand(string Code, string Name, string? Address, int TardinessToleranceMinutes = 0) : ICommand<ErrorOr<Guid>>;
