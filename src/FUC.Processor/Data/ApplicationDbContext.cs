using FUC.Common.IntegrationEventLog;
using FUC.Processor.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FUC.Processor.Data;

public class ApplicationDbContext : DbContext, IIntegrationDbContext
{
    public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }

    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
