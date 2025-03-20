using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class CoSupervisor : AuditableEntity
{
    public Guid Id { get; set; }
    public string SupervisorId { get; set; }
    public Guid TopicId { get; set; }

    public Supervisor Supervisor { get; set; } = null!;
    public Topic Topic { get; set; } = null!;
}
