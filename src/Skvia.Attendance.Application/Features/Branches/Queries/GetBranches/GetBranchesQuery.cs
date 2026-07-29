using Skvia.Attendance.Application.Features.Branches.DTOs;

namespace Skvia.Attendance.Application.Features.Branches.Queries.GetBranches;

public record GetBranchesQuery() : IQuery<ErrorOr<List<BranchResponse>>>;
