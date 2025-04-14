using FUC.Processor;
using FUC.Processor.Extensions;
using FUC.Processor.Hubs;
using FUC.Processor.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddQuartzInfrastructure();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("https://localhost:3000", "https://fu-capstone-fe.vercel.app");
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notifications");

await AppDbInitializer.SeedData(app);

await app.RunAsync();
