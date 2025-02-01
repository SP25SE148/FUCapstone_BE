using FUC.Common.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.API.Hubs;

namespace Notification.API.Consumers;

public class UsersSyncMessageConsumer : IConsumer<UsersSyncMessage>
{
    private readonly IHubContext<NotificationHub> _hubContext;   
    private readonly ILogger<UsersSyncMessageConsumer> _logger;

    public UsersSyncMessageConsumer(IHubContext<NotificationHub> hubContext, ILogger<UsersSyncMessageConsumer> logger)
    {
        _hubContext = hubContext;    
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UsersSyncMessage> context)
    {
        _logger.LogInformation("--> users sync consume in Notification service");

        await _hubContext.Clients.All.SendAsync("UsersSync", context.Message.AttempTime);
    }
}
