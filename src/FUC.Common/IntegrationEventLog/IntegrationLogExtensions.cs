using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

    public static IServiceCollection AddEventConsumerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EventConsumerConfiguration>(configuration.GetSection(nameof(EventConsumerConfiguration)));

        return services;
    }

    public static IServiceCollection AddIntegrationEventLogService<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext 
    {
        services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<TDbContext>>();

        return services;
    }
}
