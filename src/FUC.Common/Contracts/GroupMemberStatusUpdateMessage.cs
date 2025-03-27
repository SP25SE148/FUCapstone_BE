using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class GroupMemberStatusUpdateMessage : IntegrationEvent
{
    public Guid GroupMemberId { get; set; }
    public string LeaderCode { get; set; }
    public string Status { get; set; }
    public string MemberCode { get; set; }
}
