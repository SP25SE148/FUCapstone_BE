using FUC.Common.Events;

namespace FUC.Common.Contracts;

public sealed class GroupMemberCreatedEvent : IntegrationEvent
{
    public string MemberId { get; set; }
    public string LeaderId { get; set; }
    public string LeaderName { get; set; }
    public Guid GroupId { get; set; }
    public Guid GroupMemberId { get; set; }
}
