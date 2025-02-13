using FUC.Common.IntegrationEventLog;
using Microsoft.EntityFrameworkCore;

namespace FUC.Processor.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }

    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
