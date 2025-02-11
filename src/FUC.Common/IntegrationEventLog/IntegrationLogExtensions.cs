using Microsoft.EntityFrameworkCore;

namespace FUC.Common.IntegrationEventLog;

public static class IntegrationLogExtensions
{
    public static void UseIntegrationEventLogs(this ModelBuilder builder)
    {
        builder.Entity<IntegrationEventLog>(builder =>
        {
            builder.ToTable("IntegrationEventLog");

            builder.HasKey(e => e.EventId);
        });
    }
}
