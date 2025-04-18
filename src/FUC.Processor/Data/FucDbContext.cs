﻿using FUC.Common.IntegrationEventLog;
using FUC.Data.Entities;
using FUC.Processor.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FUC.Processor.Data;

public class FucDbContext : DbContext, IIntegrationDbContext
{
    public FucDbContext() { }

    public FucDbContext(DbContextOptions<FucDbContext> options) : base(options) { }

    public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }
    public DbSet<TopicAnalysis> TopicAnalysis { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FUC.Data.Data.FucDbContext).Assembly);
        modelBuilder.UseIntegrationEventLogs();
    }
}
