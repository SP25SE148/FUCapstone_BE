using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class SemanticTopicEvent : IntegrationEvent
{
    public string TopicId { get; set; }
    public List<string> SemesterIds { get; set; }
    public string ProcessedBy { get; set; }
    public bool IsCurrentSemester { get; set; }
    public string CampusId { get; set; }
    public string CapstoneId { get; set; }
}
