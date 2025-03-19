using FUC.Processor;
using FUC.Processor.Extensions;
using FUC.Processor.Hubs;
using FUC.Processor.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddQuartzInfrastructure();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notifications");

await AppDbInitializer.SeedData(app);

await app.RunAsync();
