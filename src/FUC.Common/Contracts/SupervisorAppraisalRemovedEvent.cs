using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class SupervisorAppraisalRemovedEvent : IntegrationEvent
{
    public required string SupervisorId { get; set; }
    public Guid TopicId { get; set; }
    public required string TopicEnglishName { get; set; }
}
