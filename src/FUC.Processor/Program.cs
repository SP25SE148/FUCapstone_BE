using FUC.Processor.Extensions;
using FUC.Processor.Hubs;
using FUC.Processor.Middlewares;
using FUC.Processor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddQuartzInfrastructure();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/test-mail", async (IEmailService emailService) =>
{
    await emailService.SendMailAsync("Test", "Hello", "kienltse173477@fpt.edu.vn");

    return Results.Ok();
});

app.MapHub<NotificationHub>("/notifications");

app.Run();
