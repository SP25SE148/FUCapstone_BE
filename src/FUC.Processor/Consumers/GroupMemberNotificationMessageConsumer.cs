using FUC.Common.Contracts;
using FUC.Processor.Hubs;
using FUC.Processor.Services;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace FUC.Processor.Consumers;

public class GroupMemberNotificationMessageConsumer : IConsumer<GroupMemberNotificationMessage>
{
    private readonly ILogger<GroupMemberNotificationMessage> _logger;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;
    private readonly UsersTracker _usersTracker;
    
    public GroupMemberNotificationMessageConsumer(
        ILogger<GroupMemberNotificationMessage> logger, 
        IEmailService emailService,
        IHubContext<NotificationHub, INotificationClient> hub, 
        UsersTracker usersTracker)
    {
        _logger = logger;
        _emailService = emailService;
        _hub = hub;
        _usersTracker = usersTracker;
    }
    public async Task Consume(ConsumeContext<GroupMemberNotificationMessage> context)
    { 
        _logger.LogInformation($" ---------> Consume message from processor with {context.Message.CreateBy}");

         foreach (GroupMemberNotification messageGroupMemberNotification in context.Message.GroupMemberNotifications)
         {
             List<string> connections = await _usersTracker.GetConnectionForUser(messageGroupMemberNotification.MemberEmail);
             if (connections.Any())
             {
                 _hub.Clients.Clients(connections).ReceiveAllNotifications("context.Message ");

             }
         }
        
    }
}
