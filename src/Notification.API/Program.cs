using System.Reflection;
using MassTransit;
using Notification.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x => 
{
    x.AddConsumers(Assembly.GetExecutingAssembly());

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("noti", false));
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<NotificationHub>("/notifications");

app.Run();
