using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Attendances.Queries.ExportAttendancesExcel;

public class ExportAttendancesExcelQueryHandler(
    IApplicationDbContext dbContext,
    IAttendanceExcelExporter excelExporter) : IQueryHandler<ExportAttendancesExcelQuery, ErrorOr<ExportExcelResult>>
{
    public async Task<ErrorOr<ExportExcelResult>> HandleAsync(ExportAttendancesExcelQuery query, CancellationToken cancellationToken)
    {
        var queryable = dbContext.Attendances
            .AsNoTracking()
            .Where(a => a.Date >= query.StartDate && a.Date <= query.EndDate);

        if (query.BranchId.HasValue)
        {
            queryable = queryable.Where(a => a.CheckInWorkplaceId == query.BranchId.Value);
        }

        if (query.EmployeeId.HasValue)
        {
            queryable = queryable.Where(a => a.EmployeeId == query.EmployeeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.StatusFilter))
        {
            if (query.StatusFilter.Equals("late", StringComparison.OrdinalIgnoreCase))
            {
                queryable = queryable.Where(a => a.IsLate);
            }
            else if (query.StatusFilter.Equals("onTime", StringComparison.OrdinalIgnoreCase))
            {
                queryable = queryable.Where(a => !a.IsLate);
            }
        }

        if (!string.IsNullOrWhiteSpace(query.EmployeeSearch))
        {
            var search = $"%{query.EmployeeSearch.Trim().ToLower()}%";
            queryable = queryable.Where(a =>
                EF.Functions.Like(a.Employee.FirstName.ToLower(), search) ||
                EF.Functions.Like(a.Employee.LastName.ToLower(), search) ||
                EF.Functions.Like(a.Employee.DocumentIdentifier.Number.ToLower(), search) ||
                EF.Functions.Like(a.Employee.Code.ToLower(), search));
        }

        var attendances = await queryable
            .OrderBy(a => a.Date)
            .ThenBy(a => a.Employee.LastName)
            .Select(a => new AttendanceExportDto(
                a.Date,
                a.Employee.Code,
                $"{a.Employee.LastName}, {a.Employee.FirstName}",
                a.CheckInWorkplace.Name,
                a.CheckIn,
                a.CheckOut,
                a.MinutesLate,
                a.IsLate
            ))
            .ToListAsync(cancellationToken);

        var fileContents = excelExporter.ExportAttendances(attendances, query.StartDate, query.EndDate);

        string fileName = $"Reporte_Asistencias_{query.StartDate:yyyyMMdd}_{query.EndDate:yyyyMMdd}.xlsx";
        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        return new ExportExcelResult(fileContents, contentType, fileName);
    }
}


