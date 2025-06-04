using System.Threading;
using System;
using FUC.Processor;
using FUC.Processor.Data;
using FUC.Processor.Extensions;
using FUC.Processor.Hubs;
using FUC.Processor.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

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
            .WithOrigins("https://localhost:3000", "https://fu-capstone-fe.vercel.app",
                "https://fu-capstone-fe-git-localhost-dtheng03s-projects.vercel.app");
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notifications");

app.MapGet("/test", async (ProcessorDbContext _processorDbContext) =>
{
    var currentTime = new DateTime(2025, 4, 19, 8, 0, 0).TimeOfDay;

    var buffer = TimeSpan.FromMinutes(1);

    var a = await _processorDbContext.RecurrentReminders.ToListAsync();

    var a1 = a.First(x => x.ReminderType == "GroupEvaluationProgress");

    var i = a1.RemindTime >= currentTime.Subtract(buffer);
    var c = a1.RemindTime <= currentTime.Add(buffer);

    var recurrentReminders = await _processorDbContext.RecurrentReminders
        .Where(r => r.RecurringDay == DayOfWeek.Saturday &&
                    r.RemindTime >= currentTime.Subtract(buffer) &&
                    r.RemindTime <= currentTime.Add(buffer))
        .Take(100)
        .ToListAsync();

    return recurrentReminders;
});

await AppDbInitializer.SeedData(app);

await app.RunAsync();
