using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FUC.Processor.Hubs;

public interface INotificationClient
{
    Task ReceiveAllNotifications(string message);
}

[Authorize]
public class NotificationHub : Hub<INotificationClient>
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        await Clients.All.ReceiveAllNotifications("Connected");
    }

    public async Task JoinGroup(string key)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, key);
    }
}
