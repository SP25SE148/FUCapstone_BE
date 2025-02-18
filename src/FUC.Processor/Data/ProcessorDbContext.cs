using FUC.Processor.Abstractions;
using FUC.Processor.Models;
using Microsoft.EntityFrameworkCore;

namespace FUC.Processor.Data;

public class ProcessorDbContext : DbContext, IDbContext
{
    public ProcessorDbContext() { }

    public DbSet<Reminder> Reminders { get; set; }

    public ProcessorDbContext(DbContextOptions<ProcessorDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProcessorDbContext).Assembly);
    }
}
