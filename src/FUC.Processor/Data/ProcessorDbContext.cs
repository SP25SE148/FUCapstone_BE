using FUC.Common.IntegrationEventLog;
using FUC.Processor.Abstractions;
using FUC.Processor.Models;
using Microsoft.EntityFrameworkCore;

namespace FUC.Processor.Data;

public class ProcessorDbContext : DbContext, IIntegrationDbContext
{
    public ProcessorDbContext() { }
    public ProcessorDbContext(DbContextOptions<ProcessorDbContext> options) : base(options) { }

    public DbSet<Reminder> Reminders { get; set; }
    public DbSet<RecurrentReminder> RecurrentReminders { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProcessorDbContext).Assembly);
        modelBuilder.UseIntegrationEventLogs();
    }
}
