using FUC.Data.Data;
using FUC.Data.Interceptors;
using FUC.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FUC.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();

        // Add DBContext
        services.AddDbContext<DbContext,FucDbContext>((provider, options) =>
        {
            var auditInterceptor = provider.GetService<AuditableEntityInterceptor>();
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(auditInterceptor);
        });

        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        return services;
    }
}
