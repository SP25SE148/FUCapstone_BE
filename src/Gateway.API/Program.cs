using FUC.Common.Abstractions;
using FUC.Common.Attributes;
using FUC.Common.Options;
using Gateway.API.Infrastuctures.Cache;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddStackExchangeRedisCache(redisOptions =>
{
    var connectionString = builder.Configuration.GetConnectionString("Redis");
    redisOptions.Configuration = connectionString;
});

builder.Services.AddTransient<ICacheService, CacheService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    JwtOption jwtOption = new JwtOption();
    builder.Configuration.GetSection(nameof(JwtOption)).Bind(jwtOption);

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

builder.Services.AddScoped<CustomJwtBearerEvents>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", b =>
    {
        b.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials() 
         .WithOrigins(builder.Configuration["ClientApp"]);
    });
});

var app = builder.Build();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapReverseProxy();

app.Run();
