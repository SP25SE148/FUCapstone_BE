using System.Security.Claims;
using FUC.Processor.Data;
using FUC.Processor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FUC.Processor.Hubs;
    
public interface INotificationClient
{
    Task ReceiveNotifications(ICollection<Notification> notifications);
    Task ReceiveNewNotification(string notification);
    Task NumberOfUnreadedNotifications(int count);
}

[Authorize]
public class NotificationHub : Hub<INotificationClient>
{
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorContext;

    public NotificationHub(UsersTracker usersTracker,
        ProcessorDbContext processorDbContext)
    {
        _usersTracker = usersTracker;
        _processorContext = processorDbContext; 
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        var currentUserCode = Context.User!.FindFirst(ClaimTypes.GivenName)!.Value;

        await _usersTracker.UserConnected(currentUserCode, Context.ConnectionId);
        
        var count = await _processorContext.Notifications
            .CountAsync(x => x.UserCode == currentUserCode && !x.IsRead);

        await Clients.Caller.NumberOfUnreadedNotifications(count);
    }

    public async Task GetNotifications()
    {
        var currentUserCode = Context.User!.FindFirst(ClaimTypes.GivenName)!.Value;

        var notifications = await _processorContext.Notifications
            .AsNoTracking()
            .Where(x => x.UserCode == currentUserCode)
            .OrderByDescending(x => x.CreatedDate)
            .Take(25)
            .ToListAsync();

        await _processorContext.Notifications
            .Where(x => x.UserCode == currentUserCode && !x.IsRead)
            .ExecuteUpdateAsync(x => x.SetProperty(s => s.IsRead, s => true));

        await Clients.Caller.ReceiveNotifications(notifications);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _usersTracker.UserDisconnected(Context.User!.FindFirst(ClaimTypes.GivenName)!.Value, Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }
}
