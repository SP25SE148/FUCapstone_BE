using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class GroupMemberStatusUpdateMessage : IntegrationEvent
{
    public string LeaderCode { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
}
