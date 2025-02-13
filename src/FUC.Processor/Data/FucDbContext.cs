using FUC.Common.IntegrationEventLog;
using Microsoft.EntityFrameworkCore;

namespace FUC.Processor.Data;

public class FucDbContext : DbContext
{
    public FucDbContext() { }

    public FucDbContext(DbContextOptions<FucDbContext> options) : base(options) { }

    public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }
}
