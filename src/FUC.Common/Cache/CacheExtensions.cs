using FUC.Common.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FUC.Common.Cache;

public static class CacheExtensions
{
    public static IServiceCollection AddCacheConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            redisOptions.Configuration = connectionString;
        });

        services.AddTransient<ICacheService, CacheService>();

        return services;
    }
}
