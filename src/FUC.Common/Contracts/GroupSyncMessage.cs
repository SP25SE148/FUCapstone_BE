using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class GroupSyncMessage : IntegrationEvent
{
    public string UserEmail { get; set; } // user name in identity db
    public Guid GroupId { get; set; }
    public bool IsUpdate { get; set; }
}
