using Microsoft.EntityFrameworkCore.Infrastructure;

using Skvia.Attendance.Domain.Branches;

namespace Skvia.Attendance.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<BranchUser> BranchUsers { get; }
    DbSet<Branch> Branches { get; }


    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
