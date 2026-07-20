using Skvia.Attendance.Application.Branches.DTOs;

namespace Skvia.Attendance.Application.Branches.Queries.GetBranches;

public record GetBranchesQuery() : IQuery<ErrorOr<List<GetBranchResult>>>;
