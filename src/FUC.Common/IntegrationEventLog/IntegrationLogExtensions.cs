using FUC.Common.IntegrationEventLog.BackgroundJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace FUC.Common.IntegrationEventLog;

public static class IntegrationLogExtensions
{
    public static void UseIntegrationEventLogs(this ModelBuilder builder)
    {
        builder.Entity<IntegrationEventLog>(builder =>
        {
            builder.ToTable("IntegrationEventLogs");

            builder.HasKey(e => e.Id);
        });
    }
}
