using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class NewSupervisorAssignedForTopicEvent : IntegrationEvent
{
    public required string NewSupervisorId { get; set; }
    public required string OldSupervisorId { get; set; }
    public Guid TopicId { get; set; }
    public string TopicShortName { get; set; }
}
