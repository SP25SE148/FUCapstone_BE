using Notification.API.Extensions;
using Notification.API.Hubs;
using Notification.API.Middlewares;
using Notification.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

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
