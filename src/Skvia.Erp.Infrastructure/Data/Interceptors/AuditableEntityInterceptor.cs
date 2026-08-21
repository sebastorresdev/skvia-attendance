using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.Auth.DTOs;
using Skvia.Erp.Domain.Common;

namespace Skvia.Erp.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor(ICurrentUserProvider currentUserProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        Guid? userId;
        try
        {
            // 🌟 Intentamos obtener el usuario real si existe HttpContext
            CurrentUserResponse currentUser = currentUserProvider.GetCurrentUser();
            userId = currentUser?.Id;
        }
        catch (InvalidOperationException)
        {
            // 🚀 Si cae aquí, es porque estamos en el Seeder/Migrations (Fuera de HTTP)
            // Dejamos userId como null (o un Guid quemado de sistema si tu base de datos no acepta nulls)
            userId = Guid.Empty;
        }

        DateTimeOffset now = DateTimeOffset.UtcNow;

        foreach (EntityEntry<BaseAuditableEntity> entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId ?? Guid.Empty; // O el valor de sistema
                    entry.Entity.Created = now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = userId ?? Guid.Empty;
                    entry.Entity.LastModified = now;
                    break;
            }
        }
    }
}


