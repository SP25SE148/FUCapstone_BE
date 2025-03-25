using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FUC.Processor.Data;

public class ProcessorDbContextFactory : IDesignTimeDbContextFactory<ProcessorDbContext>
{
    public ProcessorDbContext CreateDbContext(string[] args)
    {
        // Read configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Configure DbContext
        var optionsBuilder = new DbContextOptionsBuilder<ProcessorDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("ProcessorConnection"));

        return new ProcessorDbContext(optionsBuilder.Options);
    }
}

