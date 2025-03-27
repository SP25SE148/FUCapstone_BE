using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class JoinGroupRequestCreatedEvent : IntegrationEvent
{
    public string MemberCode { get; set; }

    public string LeaderCode { get; set; }
}
