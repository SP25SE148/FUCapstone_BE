using FUC.Common.Options;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Notification.API.Extensions.Options;
using Notification.API.Services;
using System.Reflection;
using System.Text;

namespace Notification.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
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
            JwtOption jwtOption = new JwtOption();
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
}
