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
        _logger.LogInformation("Starting to send notification for SupervisorAppraisalRemovedEvent with supervisor {Id}",
            message.SupervisorId);

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

        foreach (var supvisorId in message.SupervisorIds)
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
        _logger.LogInformation(
            "Starting to send notification for AssignedSupervisorForAppraisalEvent with supervisor {Id}",
            message.SupervisorId);

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

public class
    AssignedAvailableSupervisorForAppraisalEventConsumer : BaseEventConsumer<
    AssignedAvailableSupervisorForAppraisalEvent>
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
        _logger.LogInformation(
            "Starting to send notification for AssignedAvailableSupervisorForAppraisalEvent with message {Id}",
            message.Id);

        foreach (var supervisorId in message.SupervisorIds)
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

public class GroupMemberStatusUpdateMessageConsumer : BaseEventConsumer<GroupMemberStatusUpdatedEvent>
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

    protected override async Task ProcessMessage(GroupMemberStatusUpdatedEvent message)
    {
        _logger.LogInformation("Starting to send notification for GroupMemberStatusUpdateMessage with studentId {Id}",
            message.LeaderCode);

        var connections = await _usersTracker.GetConnectionForUser(message.LeaderCode);

        _processorDbContext.Notifications.Add(new Notification
        {
            UserCode = message.LeaderCode,
            ReferenceTarget = message.GroupMemberId.ToString(),
            Content = $"Member {message.MemberCode} was {message.Status} your request to join group.",
            IsRead = false,
            Type = nameof(GroupMemberStatusUpdatedEvent)
        });

        await _processorDbContext.SaveChangesAsync();

        await _hub.Clients.Clients(connections).ReceiveNewNotification
            ($"Member {message.MemberCode} was {message.Status} your request to join group.");
    }
}

public class JoinGroupRequestCreatedEventConsumer : BaseEventConsumer<JoinGroupRequestCreatedEvent>
{
    private readonly ILogger<JoinGroupRequestCreatedEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public JoinGroupRequestCreatedEventConsumer(
        ILogger<JoinGroupRequestCreatedEventConsumer> logger,
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

    protected override async Task ProcessMessage(JoinGroupRequestCreatedEvent message)
    {
        _logger.LogInformation("Starting to send notification for JoinGroupRequestCreatedEvent with studentId {Id}",
            message.LeaderCode);

        var connections = await _usersTracker.GetConnectionForUser(message.LeaderCode);

        _processorDbContext.Notifications.Add(new Notification
        {
            UserCode = message.LeaderCode,
            ReferenceTarget = string.Join("/", message.GroupId, message.JoinGroupRequestId),
            Content = $"Member {message.MemberName} - {message.MemberCode} want to join your group.",
            IsRead = false,
            Type = nameof(JoinGroupRequestCreatedEvent)
        });

        await _processorDbContext.SaveChangesAsync();

        await _hub.Clients.Clients(connections).ReceiveNewNotification
            ($"Member {message.MemberName} - {message.MemberCode} want to join your group.");
    }
}

public class GroupStatusUpdatedEventConsumer : BaseEventConsumer<GroupStatusUpdatedEvent>
{
    private readonly ILogger<GroupStatusUpdatedEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public GroupStatusUpdatedEventConsumer(
        ILogger<GroupStatusUpdatedEventConsumer> logger,
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

    protected override async Task ProcessMessage(GroupStatusUpdatedEvent message)
    {
        _logger.LogInformation("Starting to send notification for GroupStatusUpdatedEvent with groupCode {Id}",
            message.GroupCode);

        foreach (var student in message.StudentCodes)
        {
            var connections = await _usersTracker.GetConnectionForUser(student);

            _processorDbContext.Notifications.Add(new Notification
            {
                UserCode = student,
                ReferenceTarget = message.GroupId.ToString(),
                Content = $"Your group was created! GroupCode is {message.GroupCode}",
                IsRead = false,
                Type = nameof(GroupStatusUpdatedEvent)
            });

            await _processorDbContext.SaveChangesAsync();

            await _hub.Clients.Clients(connections).ReceiveNewNotification
                ($"Your group was created! GroupCode is {message.GroupCode}");
        }
    }
}

public class GroupMemberCreatedEventConsumer : BaseEventConsumer<GroupMemberCreatedEvent>
{
    private readonly ILogger<GroupMemberCreatedEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public GroupMemberCreatedEventConsumer(
        ILogger<GroupMemberCreatedEventConsumer> logger,
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

    protected override async Task ProcessMessage(GroupMemberCreatedEvent message)
    {
        _logger.LogInformation("Starting to send notification for GroupMemberCreatedEventConsumer with studentId {Id}",
            message.MemberId);

        var connections = await _usersTracker.GetConnectionForUser(message.MemberId);

        _processorDbContext.Notifications.Add(new Notification
        {
            UserCode = message.MemberId,
            ReferenceTarget = string.Join("/", message.GroupId, message.GroupMemberId),
            Content = $"Leader {message.LeaderName} - {message.LeaderId} invite you to join group.",
            IsRead = false,
            Type = nameof(GroupMemberCreatedEvent)
        });

        await _processorDbContext.SaveChangesAsync();

        await _hub.Clients.Clients(connections).ReceiveNewNotification
            ($"Leader {message.LeaderName} - {message.LeaderId} invite you to join group.");
    }
}

public class JoinGroupRequestStatusUpdatedEventConsumer : BaseEventConsumer<JoinGroupRequestStatusUpdatedEvent>
{
    private readonly ILogger<JoinGroupRequestStatusUpdatedEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public JoinGroupRequestStatusUpdatedEventConsumer(
        ILogger<JoinGroupRequestStatusUpdatedEventConsumer> logger,
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

    protected override async Task ProcessMessage(JoinGroupRequestStatusUpdatedEvent message)
    {
        _logger.LogInformation(
            "Starting to send notification for JoinGroupRequestStatusUpdatedEvent with studentId {Id}",
            message.MemberCode);

        var connections = await _usersTracker.GetConnectionForUser(message.MemberCode);

        _processorDbContext.Notifications.Add(new Notification
        {
            UserCode = message.MemberCode,
            ReferenceTarget = string.Join("/", message.GroupId, message.JoinGroupRequestId),
            Content =
                $"Leader {message.LeaderName} - {message.LeaderCode} was {message.Status} your join group request.",
            IsRead = false,
            Type = nameof(JoinGroupRequestStatusUpdatedEvent)
        });

        await _processorDbContext.SaveChangesAsync();

        await _hub.Clients.Clients(connections).ReceiveNewNotification
            ($"Leader {message.LeaderName} - {message.LeaderCode} was {message.Status} your join group request.");
    }
}

public class TopicRequestCreatedEventConsumer : BaseEventConsumer<TopicRequestCreatedEvent>
{
    private readonly ILogger<TopicRequestCreatedEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public TopicRequestCreatedEventConsumer(
        ILogger<TopicRequestCreatedEventConsumer> logger,
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

    protected override async Task ProcessMessage(TopicRequestCreatedEvent message)
    {
        _logger.LogInformation("Starting to send notification for TopicRequestCreatedEvent with Supervisor {Id}",
            message.SupervisorOfTopic);

        var connections = await _usersTracker.GetConnectionForUser(message.SupervisorOfTopic);

        _processorDbContext.Notifications.Add(new Notification
        {
            UserCode = message.SupervisorOfTopic,
            ReferenceTarget = string.Join("/", message.GroupId, message.TopicId),
            Content = $"Group {message.GroupCode} want to register your {message.TopicShortName} topic.",
            IsRead = false,
            Type = nameof(TopicRequestCreatedEvent)
        });

        await _processorDbContext.SaveChangesAsync();

        await _hub.Clients.Clients(connections).ReceiveNewNotification
            ($"Group {message.GroupCode} want to register your {message.TopicShortName} topic.");
    }
}

public class TopicRequestStatusUpdatedEventConsumer : BaseEventConsumer<TopicRequestStatusUpdatedEvent>
{
    private readonly ILogger<TopicRequestStatusUpdatedEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public TopicRequestStatusUpdatedEventConsumer(
        ILogger<TopicRequestStatusUpdatedEventConsumer> logger,
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

    protected override async Task ProcessMessage(TopicRequestStatusUpdatedEvent message)
    {
        _logger.LogInformation("Starting to send notification for TopicRequestStatusUpdatedEvent with topic {Id}",
            message.TopicId);

        foreach (var student in message.StudentCodes)
        {
            var connections = await _usersTracker.GetConnectionForUser(student);

            _processorDbContext.Notifications.Add(new Notification
            {
                UserCode = student,
                ReferenceTarget = message.TopicId.ToString(),
                Content =
                    $"Supervisor {message.SupervisorOfTopicName} has {message.Status} your registration of topic {message.TopicShortName}.",
                IsRead = false,
                Type = nameof(TopicRequestStatusUpdatedEvent)
            });

            await _processorDbContext.SaveChangesAsync();

            await _hub.Clients.Clients(connections).ReceiveNewNotification
                ($"Supervisor {message.SupervisorOfTopicName} has {message.Status} your registration of topic {message.TopicShortName}.");
        }
    }
}

public class GroupDecisionUpdatedEventConsumer : BaseEventConsumer<GroupDecisionUpdatedEvent>
{
    private readonly ILogger<GroupDecisionUpdatedEventConsumer> _logger;
    private readonly UsersTracker _usersTracker;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public GroupDecisionUpdatedEventConsumer(
        ILogger<GroupDecisionUpdatedEventConsumer> logger,
        UsersTracker usersTracker,
        IHubContext<NotificationHub, INotificationClient> hub,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _usersTracker = usersTracker;
        _logger = logger;
        _processorDbContext = processorDbContext;
        _hub = hub;
    }

    protected override async Task ProcessMessage(GroupDecisionUpdatedEvent message)
    {
        _logger.LogInformation("Starting to send notification for GroupDecisionUpdatedEvent with group {Id}",
            message.GroupId);
        foreach (var memberCode in message.MemberCode)
        {
            var connections = await _usersTracker.GetConnectionForUser(memberCode);
            var noti = new Notification
            {
                UserCode = memberCode,
                ReferenceTarget = message.GroupId.ToString(),
                Content = $"Group {message.GroupCode} was decided to  {message.Decision} by your mentor.",
                IsRead = false,
                Type = nameof(GroupDecisionUpdatedEvent)
            };
            _processorDbContext.Notifications.Add(noti);
            await _processorDbContext.SaveChangesAsync();
            await _hub.Clients.Clients(connections).ReceiveNewNotification(noti.Content);
        }
    }
}
