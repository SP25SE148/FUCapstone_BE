using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class GroupDecisionUpdatedEvent : IntegrationEvent
{
    public Guid GroupId { get; set; }
    public string GroupCode { get; set; }
    public string Decision { get; set; }
    public List<string> MemberCode { get; set; }
}
