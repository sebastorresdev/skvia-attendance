using Microsoft.Extensions.Diagnostics.HealthChecks;
using Skvia.Attendance.Infrastructure.Data;

namespace Skvia.Attendance.Api.Common.Health;

public class DatabaseHealthCheck(ApplicationDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("La conexión a la base de datos es correcta.")
                : HealthCheckResult.Unhealthy("No se pudo conectar a la base de datos.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Error al verificar la salud de la base de datos.", ex);
        }
    }
}
