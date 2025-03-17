using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class JoinGroupRequestExpirationEvent : IntegrationEvent
{
    public Guid JoinGroupRequestId { get; set; }
}

public class GroupMemberExpirationEvent : IntegrationEvent
{
    public Guid GroupMemberId { get; set; }
}
