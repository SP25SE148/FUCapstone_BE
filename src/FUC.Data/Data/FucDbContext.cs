using FUC.Common.IntegrationEventLog;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUC.Data.Data;

public class FucDbContext : DbContext
{
    public bool DisableInterceptors { get; set; } = false;
    public FucDbContext() { }

    public FucDbContext(DbContextOptions<FucDbContext> options) : base(options) { }

    public DbSet<Campus> Campuses { get; set; }

    public DbSet<Capstone> Capstones { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<GroupMember> GroupMembers { get; set; }

    public DbSet<Major> Majors { get; set; }

    public DbSet<MajorGroup> MajorGroups { get; set; }

    public DbSet<Semester> Semesters { get; set; }

    public DbSet<Student> Students { get; set; }

    public DbSet<Supervisor> Supervisors { get; set; }

    public DbSet<Topic> Topics { get; set; }

    public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine).EnableSensitiveDataLogging();
        optionsBuilder.UseNpgsql("Server=localhost:5432; User Id=postgres;Password=postgrespw;Database=fuc");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FucDbContext).Assembly);
        modelBuilder.UseIntegrationEventLogs();
    }
}
