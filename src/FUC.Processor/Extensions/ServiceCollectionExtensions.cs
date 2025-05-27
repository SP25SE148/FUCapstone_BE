using FUC.Common.Cache;
using FUC.Common.IntegrationEventLog;
using FUC.Common.IntegrationEventLog.BackgroundJobs;
using FUC.Common.Options;
using FUC.Processor.Abstractions;
using FUC.Processor.Data;
using FUC.Processor.Extensions.Options;
using FUC.Processor.Hubs;
using FUC.Processor.Jobs;
using FUC.Processor.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Refit;
using System.Reflection;
using System.Security.Authentication;
using System.Text;

namespace FUC.Processor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ProcessorDbContext>((provider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ProcessorConnection"));
        });

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

            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("processor", false));
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"], 15672, "/", host =>
                {
                    host.Username(configuration.GetValue("RabbitMq:Username", "guest"));
                    host.Password(configuration.GetValue("RabbitMq:Password", "guest"));
                });

                cfg.ConfigureEndpoints(context);
            });
        });


        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        services.AddScoped<IEmailService, EmailSerivce>();


        services.AddRefitClient<ISemanticApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:9000"));

        services.AddSingleton<UsersTracker>();

        services.AddCacheConfiguration(configuration);

        // DI IntegrationEventLog
        services.AddEventConsumerConfiguration(configuration);
        services.AddIntegrationEventLogService<ProcessorDbContext>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
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
        var integrationDbContextTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass &&
                        !t.IsAbstract &&
                        typeof(DbContext).IsAssignableFrom(t) &&
                        typeof(IIntegrationDbContext).IsAssignableFrom(t))
            .ToList();

        services.AddQuartz(configure =>
        {
            // Add IntegrationEventJobs
            foreach (var dbContextType in integrationDbContextTypes)
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

            // Add ReminderJobs
            var reminderJobKey = new JobKey(nameof(ProcessRemindersJob));

            configure
                .AddJob(typeof(ProcessRemindersJob), reminderJobKey)
                .AddTrigger(trigger =>
                    trigger.ForJob(reminderJobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithInterval(TimeSpan.FromSeconds(30)) // Run every 10 sec
                                .RepeatForever()));
        });

        // Register Quartz Hosted Service
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = false);

        return services;
    }
}
