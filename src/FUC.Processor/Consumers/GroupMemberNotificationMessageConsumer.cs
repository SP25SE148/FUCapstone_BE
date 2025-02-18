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
    
    public GroupMemberNotificationMessageConsumer(
        ILogger<GroupMemberNotificationMessage> logger, 
        IEmailService emailService,
        IHubContext<NotificationHub, INotificationClient> hub)
    {
        _logger = logger;
        _emailService = emailService;
        _hub = hub;
    }
    public async Task Consume(ConsumeContext<GroupMemberNotificationMessage> context)
    { 
        _logger.LogInformation($" ---------> Consume message from processor with {context.Message.CreateBy}");
    }
}
