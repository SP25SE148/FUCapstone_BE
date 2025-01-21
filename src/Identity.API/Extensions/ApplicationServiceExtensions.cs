using System.Text;
using Identity.API.Attributes;
using Identity.API.Data;
using Identity.API.Extensions.Options;
using Identity.API.Infrastuctures.Authentication;
using Identity.API.Infrastuctures.Cache;
using Identity.API.Interfaces;
using Identity.API.Mapper;
using Identity.API.Models;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddAutoMapper(typeof(ServiceProfiles));

        services.AddMassTransit(x => {

            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddCors();

        return services;
    }

    public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ áàảãạâấầẩẫậăắằẳẵặéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđ";

                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            JwtOption jwtOption = new JwtOption();
            configuration.GetSection(nameof(JwtOption)).Bind(jwtOption);

            o.SaveToken = true; // Save token into AuthenticationProperties

            var Key = Encoding.UTF8.GetBytes(jwtOption.SecretKey);
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, // on production make it true
                ValidateAudience = true, // on production make it true
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOption.Issuer,
                ValidAudience = jwtOption.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            o.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                    }
                    return Task.CompletedTask;
                }
            };

            o.EventsType = typeof(CustomJwtBearerEvents);
        });

        services.AddAuthorization();
        services.AddScoped<CustomJwtBearerEvents>();
    }

    public static void AddServicesInfrastructure(this IServiceCollection services)
        => services.AddTransient<IJwtTokenService, JwtTokenService>()
            .AddTransient<ICacheService, CacheService>();

    public static void AddRedisInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // khi sử dụng Cace - ta sử dụng IDistributedCache _distributedCache trong class CacheService
        // IDistributedCache nó hay ở chỗ nếu ta không cấu hình gì hết thì mặc định nó sẽ là MemoryCache
        //Nhưng ở đây mình đã cấu hình thằng Redis với AddStackExchangeRedisCache() thì nó sẽ trở thành DistributeCache with Redis
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            redisOptions.Configuration = connectionString;
        });
    }
}
