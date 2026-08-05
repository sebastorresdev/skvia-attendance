namespace Skvia.Attendance.Application.Features.Attendances.Queries.GetAttendances;

public class GetAttendancesQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetAttendancesQuery, ErrorOr<List<AttendanceResponse>>>
{
    public async Task<ErrorOr<List<AttendanceResponse>>> HandleAsync(GetAttendancesQuery query, CancellationToken cancellationToken)
    {
        var queryable = dbContext.Attendances
            .Include(a => a.Employee)
            .Include(a => a.CheckInBranch)
            .AsNoTracking()
            .Where(a => a.Date >= query.StartDate && a.Date <= query.EndDate);

        if (query.BranchId.HasValue)
        {
            queryable = queryable.Where(a => a.CheckInBranchId == query.BranchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.EmployeeSearch))
        {
            var search = query.EmployeeSearch.Trim().ToLower();
            queryable = queryable.Where(a =>
                a.Employee.FirstName.ToLower().Contains(search) ||
                a.Employee.LastName.ToLower().Contains(search) ||
                a.Employee.DocumentIdentifier.Number.ToLower().Contains(search) ||
                a.Employee.Code.ToLower().Contains(search));
        }

        var attendances = await queryable
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Employee.LastName)
            .ToListAsync(cancellationToken);

        var responseList = new List<AttendanceResponse>();

        foreach (var attendance in attendances)
        {
            // Calculate Tardiness
            int tardinessMinutes = 0;
            bool isLate = false;

            // For now, we will just use the stored MinutesLate since tolerance is applied at Check-in
            isLate = attendance.IsLate;
            tardinessMinutes = attendance.MinutesLate;

            responseList.Add(new AttendanceResponse(
                attendance.Id,
                attendance.EmployeeId,
                $"{attendance.Employee.FirstName} {attendance.Employee.LastName}",
                attendance.Employee.Code,
                attendance.CheckInBranchId,
                attendance.CheckInBranch.Name,
                attendance.Date,
                null, // assignedStartTime
                null, // assignedEndTime
                attendance.CheckIn,
                attendance.CheckOut,
                tardinessMinutes,
                isLate
            ));
        }

        return responseList;
    }
}
