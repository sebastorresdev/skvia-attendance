namespace Skvia.Erp.Application.Features.Attendances.Queries.GetMonthlySummary;

public record EmployeeMonthlySummaryItemDto(
    Guid EmployeeId,
    string EmployeeCode,
    string EmployeeName,
    string BranchName,
    int TotalWorkDaysScheduled,
    int DaysWorked,
    int DaysOff,
    int VacationDays,
    int MedicalLeaveDays,
    int UnjustifiedAbsences,
    int JustifiedAbsences,
    int TotalLateMinutes,
    int JustifiedLateMinutes,
    int TotalOvertimeMinutes);

public record MonthlySummaryResponse(
    int Year,
    int Month,
    int TotalEmployees,
    List<EmployeeMonthlySummaryItemDto> Items);

