using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class SemanticProcessingForCreatedTopicEvent : IntegrationEvent
{
    public string TopicId { get; set; }
}
