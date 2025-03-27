using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Processor.Data;
using FUC.Processor.Hubs;
using FUC.Processor.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class SupervisorAppraisalRemovedEventConsumer : BaseEventConsumer<SupervisorAppraisalRemovedEvent>
{
    private readonly ILogger<SupervisorAppraisalRemovedEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public SupervisorAppraisalRemovedEventConsumer(ILogger<SupervisorAppraisalRemovedEventConsumer> logger,
        UsersTracker usersTracker,
        IHubContext<NotificationHub, INotificationClient> hub,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _usersTracker = usersTracker;
        _hub = hub; 
        _processorDbContext = processorDbContext;   
    }

    protected override async Task ProcessMessage(SupervisorAppraisalRemovedEvent message)
    {
        _logger.LogInformation("Starting to send notification for SupervisorAppraisalRemovedEvent with supervisor {Id}", message.SupervisorId);

        var connections = await _usersTracker.GetConnectionForUser(message.SupervisorId);

        _processorDbContext.Notifications.Add(new Notification 
        { 
            UserCode = message.SupervisorId,
            ReferenceTarget = message.TopicId.ToString(),
            Content = $"You was removed for appraisal of Topic: {message.TopicEnglishName}",
            IsRead = false,
            Type = nameof(SupervisorAppraisalRemovedEvent)
        });

        await _processorDbContext.SaveChangesAsync();

        await _hub.Clients.Clients(connections).ReceiveNewNotification
            ($"You was removed for appraisal of Topic: {message.TopicEnglishName}");
    }
}

public class ReAssignAppraisalTopicEventConsumer : BaseEventConsumer<ReAssignAppraisalTopicEvent>
{
    private readonly ILogger<ReAssignAppraisalTopicEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public ReAssignAppraisalTopicEventConsumer(ILogger<ReAssignAppraisalTopicEventConsumer> logger,
        UsersTracker usersTracker,
        IHubContext<NotificationHub, INotificationClient> hub,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _usersTracker = usersTracker;
        _hub = hub;
        _processorDbContext = processorDbContext;
    }

    protected override async Task ProcessMessage(ReAssignAppraisalTopicEvent message)
    {
        _logger.LogInformation("Starting to send notification for ReAssignAppraisalTopicEvent with topic {Id}",
            message.TopicId);

        var supervisorsConnections = new List<string>();
        
        foreach(var supvisorId in message.SupervisorIds)
        {
            var connections = await _usersTracker.GetConnectionForUser(supvisorId);

            supervisorsConnections.AddRange(connections);

            _processorDbContext.Notifications.Add(new Notification
            {
                UserCode = supvisorId,
                ReferenceTarget = message.TopicId.ToString(),
                Content = $"You have to re-appraisal for Topic: {message.TopicEnglishName}",
                IsRead = false,
                Type = nameof(ReAssignAppraisalTopicEvent)
            });
        }

        await _processorDbContext.SaveChangesAsync();

        await _hub.Clients.Clients(supervisorsConnections).ReceiveNewNotification
            ($"You have to re-appraisal for Topic: {message.TopicEnglishName}");
    }
}

public class TopicApprovedEventConsumer : BaseEventConsumer<TopicApprovedEvent>
{
    private readonly ILogger<TopicApprovedEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public TopicApprovedEventConsumer(ILogger<TopicApprovedEventConsumer> logger,
        UsersTracker usersTracker,
        IHubContext<NotificationHub, INotificationClient> hub,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _usersTracker = usersTracker;
        _hub = hub;
        _processorDbContext = processorDbContext;
    }

    protected override async Task ProcessMessage(TopicApprovedEvent message)
    {
        _logger.LogInformation("Starting to send notification for TopicApprovedEvent with topic {Id}", message.TopicId);

        var connections = await _usersTracker.GetConnectionForUser(message.SupervisorId);

        _processorDbContext.Notifications.Add(new Notification
        {
            UserCode = message.SupervisorId,
            ReferenceTarget = $"{message.TopicId}/{message.TopicCode}",
            Content = $"Your topic {message.TopicEnglishName} was approved. TopicCode is {message.TopicCode}",
            IsRead = false,
            Type = nameof(TopicApprovedEvent)
        });

        await _processorDbContext.SaveChangesAsync();

        await _hub.Clients.Clients(connections).ReceiveNewNotification
            ($"Your topic {message.TopicEnglishName} was approved. TopicCode is {message.TopicCode}");
    }
}

public class AssignedSupervisorForAppraisalEventConsumer : BaseEventConsumer<AssignedSupervisorForAppraisalEvent>
{
    private readonly ILogger<AssignedSupervisorForAppraisalEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public AssignedSupervisorForAppraisalEventConsumer(ILogger<AssignedSupervisorForAppraisalEventConsumer> logger,
        UsersTracker usersTracker,
        IHubContext<NotificationHub, INotificationClient> hub,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _usersTracker = usersTracker;
        _hub = hub;
        _processorDbContext = processorDbContext;
    }

    protected override async Task ProcessMessage(AssignedSupervisorForAppraisalEvent message)
    {
        _logger.LogInformation("Starting to send notification for AssignedSupervisorForAppraisalEvent with supervisor {Id}", message.SupervisorId);

        var connections = await _usersTracker.GetConnectionForUser(message.SupervisorId);

        _processorDbContext.Notifications.Add(new Notification
        {
            UserCode = message.SupervisorId,
            ReferenceTarget = message.TopicId.ToString(),
            Content = $"You was assigned for appraisal of Topic: {message.TopicEnglishName}",
            IsRead = false,
            Type = nameof(AssignedSupervisorForAppraisalEvent)
        });

        await _processorDbContext.SaveChangesAsync();

        await _hub.Clients.Clients(connections).ReceiveNewNotification
            ($"You was assigned for appraisal of Topic: {message.TopicEnglishName}");
    }
}

public class AssignedAvailableSupervisorForAppraisalEventConsumer : BaseEventConsumer<AssignedAvailableSupervisorForAppraisalEvent>
{
    private readonly ILogger<AssignedAvailableSupervisorForAppraisalEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public AssignedAvailableSupervisorForAppraisalEventConsumer(
        ILogger<AssignedAvailableSupervisorForAppraisalEventConsumer> logger,
        UsersTracker usersTracker,
        IHubContext<NotificationHub, INotificationClient> hub,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _usersTracker = usersTracker;
        _hub = hub;
        _processorDbContext = processorDbContext;
    }

    protected override async Task ProcessMessage(AssignedAvailableSupervisorForAppraisalEvent message)
    {
        _logger.LogInformation("Starting to send notification for AssignedAvailableSupervisorForAppraisalEvent with message {Id}", message.Id);

        foreach(var supervisorId in message.SupervisorIds)
        {
            var connections = await _usersTracker.GetConnectionForUser(supervisorId);

            _processorDbContext.Notifications.Add(new Notification
            {
                UserCode = supervisorId,
                ReferenceTarget = string.Empty,
                Content = "You have some topic which need to your appraisal.",
                IsRead = false,
                Type = nameof(AssignedAvailableSupervisorForAppraisalEvent)
            });

            await _processorDbContext.SaveChangesAsync();

            await _hub.Clients.Clients(connections).ReceiveNewNotification
                ("You have some topic which need to your appraisal.");
        }
    }
}

public class GroupMemberStatusUpdateMessageConsumer : BaseEventConsumer<GroupMemberStatusUpdateMessage>
{
    private readonly ILogger<GroupMemberStatusUpdateMessageConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public GroupMemberStatusUpdateMessageConsumer(
        ILogger<GroupMemberStatusUpdateMessageConsumer> logger,
        UsersTracker usersTracker,
        IHubContext<NotificationHub, INotificationClient> hub,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _usersTracker = usersTracker;
        _hub = hub;
        _processorDbContext = processorDbContext;
    }

    protected override async Task ProcessMessage(GroupMemberStatusUpdateMessage message)
    {
        _logger.LogInformation("Starting to send notification for GroupMemberStatusUpdateMessage with studentId {Id}", message.LeaderCode);

        var connections = await _usersTracker.GetConnectionForUser(message.LeaderCode);

        _processorDbContext.Notifications.Add(new Notification
        {
            UserCode = message.LeaderCode,
            ReferenceTarget = message.GroupMemberId.ToString(),
            Content = $"Member {message.MemberCode} was {message.Status} your request to join group.",
            IsRead = false,
            Type = nameof(GroupMemberStatusUpdateMessage)
        });

        await _processorDbContext.SaveChangesAsync();

        await _hub.Clients.Clients(connections).ReceiveNewNotification
            ($"Member {message.MemberCode} was {message.Status} your request to join group.");
    }
}
