using FUC.Data.Data;
using FUC.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FUC.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DBContext
        services.AddDbContext<FucDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
