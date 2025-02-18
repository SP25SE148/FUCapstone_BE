using FUC.Common.Contracts;
using FUC.Processor.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace FUC.Processor.Consumers;

public class GroupMemberStatusUpdateMessageConsumer : IConsumer<GroupMemberStatusUpdateMessage>
{
    private readonly ILogger<GroupMemberStatusUpdateMessage> _logger;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public GroupMemberStatusUpdateMessageConsumer(ILogger<GroupMemberStatusUpdateMessage> logger, IHubContext<NotificationHub, INotificationClient> hub)
    {
        _logger = logger;
        _hub = hub;
    }

    public async Task Consume(ConsumeContext<GroupMemberStatusUpdateMessage> context)
    {
        _logger.LogInformation($"---------> Consume Group Member Status Update Message from FUC.Processor");
    }
}
