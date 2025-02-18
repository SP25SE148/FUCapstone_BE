using FUC.Common.IntegrationEventLog;
using FUC.Processor.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FUC.Processor.Data;

public class FucDbContext : DbContext, IIntegrationDbContext
{
    public FucDbContext() { }

    public FucDbContext(DbContextOptions<FucDbContext> options) : base(options) { }

    public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }
}
