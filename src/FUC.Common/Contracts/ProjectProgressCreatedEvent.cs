using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class ProjectProgressCreatedEvent : IntegrationEvent
{
    public Guid GroupId { get; set; }
    public List<string> StudentCodes { get; set; }
    public required string Type { get; set; }
}
