﻿using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class TopicRequestExpirationEvent : IntegrationEvent
{
    public Guid TopicRequestId { get; set; }
}

public sealed class TopicRequestCreatedEvent : IntegrationEvent
{
    public Guid TopicId { get; set; }
    public string GroupCode { get; set; }
    public Guid GroupId { get; set; }
    public string SupervisorOfTopic { get; set; }
    public string TopicShortName { get; set; }
}

public sealed class TopicRequestStatusUpdatedEvent : IntegrationEvent
{
    public Guid TopicId { get; set; }
    public List<string> StudentCodes { get; set; }
    public string SupervisorOfTopicName { get; set; }
    public string TopicShortName { get; set; }
    public string Status { get; set; }
}
