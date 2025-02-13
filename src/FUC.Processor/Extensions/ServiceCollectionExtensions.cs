using FUC.Common.IntegrationEventLog.BackgroundJobs;
using FUC.Common.Options;
using FUC.Processor.Data;
using FUC.Processor.Extensions.Options;
using FUC.Processor.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using System.Reflection;
using System.Text;

namespace FUC.Processor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FucDbContext>((provider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("FucConnection"));
        });

        services.AddDbContext<ApplicationDbContext>((provider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("IdentityConnection"));
        });

        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());

            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("noti", false));
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        services.AddScoped<IEmailService, EmailSerivce>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            var jwtOption = new JwtOption();
            configuration.GetSection(nameof(JwtOption)).Bind(jwtOption);

            o.SaveToken = true; // Save token into AuthenticationProperties

            var Key = Encoding.UTF8.GetBytes(jwtOption.SecretKey);
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false, // on production make it true
                ValidateAudience = false, // on production make it true
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                //ValidIssuer = jwtOption.Issuer,
                //ValidAudience = jwtOption.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notifications"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddQuartzInfrastructure(this IServiceCollection services)
    {
        var dbContextTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(DbContext).IsAssignableFrom(t))
            .ToList();

        services.AddQuartz(configure =>
        {
            foreach (var dbContextType in dbContextTypes)
            {
                var dbContextName = dbContextType.Name;

                var jobKey = new JobKey($"ProcessIntegrationEventsJob-{dbContextName}");

                var jobType = typeof(ProcessIntegrationEventsJob<>).MakeGenericType(dbContextType);

                configure
                    .AddJob(jobType, jobKey)
                    .AddTrigger(trigger =>
                        trigger.ForJob(jobKey)
                            .WithSimpleSchedule(schedule =>
                                schedule.WithInterval(TimeSpan.FromSeconds(10)) // Run every 10 sec
                                        .RepeatForever()));
            }
        });

        // Register Quartz Hosted Service
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = false);

        return services;
    }
}
