using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class TopicRequestExpirationEvent : IntegrationEvent
{
    public Guid TopicRequestId { get; set; }
}
