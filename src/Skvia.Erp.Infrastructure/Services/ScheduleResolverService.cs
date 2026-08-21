using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.EmployeeSchedules.DTOs;
using Skvia.Erp.Domain.EmployeeSchedules;

namespace Skvia.Erp.Infrastructure.Services;

public class ScheduleResolverService(IApplicationDbContext dbContext) : IScheduleResolverService
{
    public async Task<ResolvedScheduleDayDto?> ResolveForDayAsync(
        Guid employeeId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var result = await ResolveRangeAsync(employeeId, date, date, cancellationToken);
        return result.FirstOrDefault();
    }

    public async Task<List<ResolvedScheduleDayDto>> ResolveRangeAsync(
        Guid employeeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        var grid = await ResolveGridAsync([employeeId], startDate, endDate, cancellationToken);
        return grid.TryGetValue(employeeId, out var list) ? list : [];
    }

    public async Task<Dictionary<Guid, List<ResolvedScheduleDayDto>>> ResolveGridAsync(
        List<Guid> employeeIds,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        if (employeeIds.Count == 0 || endDate < startDate)
            return [];

        // 1. Cargar excepciones para los empleados en el rango
        var exceptions = await dbContext.ScheduleExceptions
            .AsNoTracking()
            .Include(se => se.CustomSchedule)
            .Where(se => employeeIds.Contains(se.EmployeeId) && se.Date >= startDate && se.Date <= endDate)
            .ToListAsync(cancellationToken);

        // 2. Cargar asignaciones de horario base en el rango
        var baseAssignments = await dbContext.EmployeeSchedules
            .AsNoTracking()
            .Include(es => es.Schedule)
            .Where(es => employeeIds.Contains(es.EmployeeId) &&
                         es.EffectiveFrom <= endDate &&
                         (es.EffectiveTo == null || es.EffectiveTo >= startDate))
            .OrderByDescending(es => es.EffectiveFrom)
            .ToListAsync(cancellationToken);

        // Mapas indexados para búsqueda rápida en memoria
        var exceptionsMap = exceptions
            .GroupBy(e => (e.EmployeeId, e.Date))
            .ToDictionary(g => g.Key, g => g.First());

        var baseAssignmentsMap = baseAssignments
            .GroupBy(b => b.EmployeeId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var resultGrid = new Dictionary<Guid, List<ResolvedScheduleDayDto>>();

        foreach (var empId in employeeIds)
        {
            var resolvedDays = new List<ResolvedScheduleDayDto>();
            var empBaseAssignments = baseAssignmentsMap.GetValueOrDefault(empId) ?? [];

            for (var currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                // A. Verificar si existe una Excepción
                if (exceptionsMap.TryGetValue((empId, currentDate), out var exception))
                {
                    TimeOnly? startTime = exception.StartTime;
                    TimeOnly? endTime = exception.EndTime;
                    bool hasBreak = false;
                    TimeOnly? breakStart = null;
                    TimeOnly? breakEnd = null;
                    Guid? schedId = exception.CustomScheduleId;
                    string? schedCode = null;
                    string? schedDesc = null;

                    if (exception.CustomSchedule is not null)
                    {
                        startTime ??= exception.CustomSchedule.DefaultStartTime;
                        endTime ??= exception.CustomSchedule.DefaultEndTime;
                        hasBreak = exception.CustomSchedule.HasBreak;
                        breakStart = exception.CustomSchedule.BreakStartTime;
                        breakEnd = exception.CustomSchedule.BreakEndTime;
                        schedCode = exception.CustomSchedule.Code;
                        schedDesc = exception.CustomSchedule.Description;
                    }

                    resolvedDays.Add(new ResolvedScheduleDayDto(
                        EmployeeId: empId,
                        Date: currentDate,
                        DayType: exception.DayType,
                        StartTime: exception.IsDayOff ? null : startTime,
                        EndTime: exception.IsDayOff ? null : endTime,
                        HasBreak: hasBreak,
                        BreakStartTime: breakStart,
                        BreakEndTime: breakEnd,
                        ScheduleId: schedId,
                        ScheduleCode: schedCode,
                        ScheduleDescription: schedDesc,
                        IsException: true,
                        ExceptionId: exception.Id,
                        ExceptionReason: exception.Reason));

                    continue;
                }

                // B. Si no hay excepción, resolver con Horario Base activo
                var activeBase = empBaseAssignments.FirstOrDefault(b =>
                    b.EffectiveFrom <= currentDate && (b.EffectiveTo == null || b.EffectiveTo >= currentDate));

                if (activeBase is not null)
                {
                    // Si viene con una plantilla asignada (Schedule)
                    if (activeBase.Schedule is not null)
                    {
                        resolvedDays.Add(new ResolvedScheduleDayDto(
                            EmployeeId: empId,
                            Date: currentDate,
                            DayType: ScheduleDayType.WorkDay,
                            StartTime: activeBase.Schedule.DefaultStartTime,
                            EndTime: activeBase.Schedule.DefaultEndTime,
                            HasBreak: activeBase.Schedule.HasBreak,
                            BreakStartTime: activeBase.Schedule.BreakStartTime,
                            BreakEndTime: activeBase.Schedule.BreakEndTime,
                            ScheduleId: activeBase.Schedule.Id,
                            ScheduleCode: activeBase.Schedule.Code,
                            ScheduleDescription: activeBase.Schedule.Description,
                            IsException: false,
                            ExceptionId: null,
                            ExceptionReason: null));

                        continue;
                    }

                    // Si es un registro individual legado
                    if (activeBase.Date == currentDate)
                    {
                        resolvedDays.Add(new ResolvedScheduleDayDto(
                            EmployeeId: empId,
                            Date: currentDate,
                            DayType: activeBase.DayType,
                            StartTime: activeBase.AssignedStartTime,
                            EndTime: activeBase.AssignedEndTime,
                            HasBreak: false,
                            BreakStartTime: null,
                            BreakEndTime: null,
                            ScheduleId: activeBase.BaseScheduleId,
                            ScheduleCode: null,
                            ScheduleDescription: null,
                            IsException: false,
                            ExceptionId: null,
                            ExceptionReason: null));

                        continue;
                    }
                }

                // C. Si no hay ni plantilla base ni excepción -> Día No Programado / Descanso
                resolvedDays.Add(new ResolvedScheduleDayDto(
                    EmployeeId: empId,
                    Date: currentDate,
                    DayType: ScheduleDayType.DayOff,
                    StartTime: null,
                    EndTime: null,
                    HasBreak: false,
                    BreakStartTime: null,
                    BreakEndTime: null,
                    ScheduleId: null,
                    ScheduleCode: null,
                    ScheduleDescription: null,
                    IsException: false,
                    ExceptionId: null,
                    ExceptionReason: null));
            }

            resultGrid[empId] = resolvedDays;
        }

        return resultGrid;
    }
}

