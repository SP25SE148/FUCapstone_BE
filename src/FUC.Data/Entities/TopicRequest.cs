using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public class TopicRequest : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public string SupervisorId { get; set; }
    public Guid GroupId { get; set; }
    public Guid TopicId { get; set; }
    public TopicRequestStatus Status { get; set; }

    public Group Group { get; set; } = null!;

    public Topic Topic { get; set; } = null!;

    public Supervisor Supervisor { get; set; } = null!;
}
