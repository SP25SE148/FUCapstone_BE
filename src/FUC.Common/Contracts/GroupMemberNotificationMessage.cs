using FUC.Common.Events;

namespace FUC.Common.Contracts;

public sealed class GroupMemberNotificationMessage : IntegrationEvent
{
    public int AttemptTime { get; set; }
    public string CreateBy { get; set; }
    public string LeaderName { get; set; }
    public string LeaderEmail { get; set; }
    public IEnumerable<GroupMemberNotification> GroupMemberNotifications { get; set; }
}
