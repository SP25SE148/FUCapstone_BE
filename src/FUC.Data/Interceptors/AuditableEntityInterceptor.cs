﻿using FUC.Common.Abstractions;
using FUC.Data.Abstractions;
using FUC.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FUC.Data.Interceptors;
public class AuditableEntityInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is FucDbContext dbContext && dbContext.DisableInterceptors)
        {
            return base.SavingChanges(eventData, result); 
        }

        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
      InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is FucDbContext dbContext && dbContext.DisableInterceptors)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null)
            return;

        var now = DateTime.Now;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                    {
                        entry.Entity.CreatedBy = currentUser.Email;
                    }
                    if (entry.Entity.CreatedDate == default)
                    {
                        entry.Entity.CreatedDate = now;
                    }
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedBy = currentUser.Email;
                    entry.Entity.UpdatedDate = now;
                    break;
            }
        }
    }
}
