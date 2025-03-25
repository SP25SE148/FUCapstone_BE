using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class ReAssignAppraisalTopicEvent : IntegrationEvent
{
    public List<string> SupervisorIds { get; set; }
    public Guid TopicId { get; set; }
    public string TopicEnglishName { get; set; }
}
