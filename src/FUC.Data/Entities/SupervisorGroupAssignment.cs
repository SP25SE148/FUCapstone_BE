using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class SupervisorGroupAssignment : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string SupervisorId { get; set; }
    public string? Supervisor2Id { get; set; }

    public Group Group { get; set; } = null!;
    public Supervisor Supervisor { get; set; } = null!;
    public Supervisor? Supervisor2 { get; set; }
}
