using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class GroupStatusUpdatedEvent : IntegrationEvent
{
    public string GroupCode { get; set; }
    public List<string> StudentCodes{ get; set; }
    public Guid GroupId { get; set; }
}
