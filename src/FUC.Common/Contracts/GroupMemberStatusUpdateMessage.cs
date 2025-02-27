using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class GroupMemberStatusUpdateMessage : IntegrationEvent
{
    public int AttemptTime { get; set; }
    public string CreatedBy { get; set; }
    public string Status { get; set; }
}
