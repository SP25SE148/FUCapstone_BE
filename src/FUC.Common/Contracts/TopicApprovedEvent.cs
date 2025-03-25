using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class TopicApprovedEvent : IntegrationEvent
{
    public Guid TopicId { get; set; }
    public string SupervisorId { get; set; }
    public string TopicEnglishName { get; set; }
    public string TopicCode { get; set; }
}

public class AssignedSupervisorForAppraisalEvent : IntegrationEvent
{
    public Guid TopicId { get; set; }
    public string SupervisorId { get; set; }
    public string TopicEnglishName { get; set; }
}

public class AssignedAvailableSupervisorForAppraisalEvent : IntegrationEvent
{
    public List<string> SupervisorIds { get; set; }
}
