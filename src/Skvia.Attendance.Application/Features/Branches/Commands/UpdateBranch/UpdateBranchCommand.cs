namespace Skvia.Attendance.Application.Features.Branches.Commands.UpdateBranch;

public record UpdateBranchCommand(Guid BranchId, string Code, string Name, string? Address, int TardinessToleranceMinutes = 0) : ICommand<ErrorOr<Success>>;
