using FUC.Common.Events;

namespace FUC.Common.Contracts;

public sealed class JoinGroupRequestCreatedEvent : IntegrationEvent
{
    public string MemberCode { get; set; }
    public string MemberName { get; set; }
    public string LeaderCode { get; set; }
    public Guid GroupId { get; set; }
    public Guid JoinGroupRequestId { get; set; }
}

public sealed class JoinGroupRequestStatusUpdatedEvent : IntegrationEvent
{
    public string MemberCode { get; set; }
    public string LeaderName { get; set; }
    public string LeaderCode { get; set; }
    public Guid GroupId { get; set; }
    public Guid JoinGroupRequestId { get; set; }
    public string Status { get; set; }
}
