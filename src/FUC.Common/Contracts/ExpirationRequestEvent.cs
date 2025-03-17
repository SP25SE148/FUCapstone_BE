using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class ExpirationRequestEvent : IntegrationEvent
{
    public Guid RequestId { get; set; } // represent for TopicRequest/JoinGroupRequest/GroupMemberRequest
    public required string RequestType { get; set; }
    public TimeSpan ExpirationDuration { get; set; }
}
