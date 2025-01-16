using Microsoft.EntityFrameworkCore;

namespace FUC.Data.Data;

public class FucDbContext : DbContext
{
    public FucDbContext()
    {
    }

    public FucDbContext(DbContextOptions<FucDbContext> options) : base(options)
    {
    }

    /*
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Server=localhost:5432;User Id=postgres;Password=postgrespw;Database=fuc");
    }*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FucDbContext).Assembly);
    }
}
