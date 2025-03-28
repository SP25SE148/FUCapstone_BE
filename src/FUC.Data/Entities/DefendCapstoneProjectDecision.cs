using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public sealed class DefendCapstoneProjectDecision : AuditableEntity
{
    public Guid Id { get; set; }
    public string SupervisorId { get; set; }
    public Guid GroupId { get; set; }
    public string? Comment { get; set; }
    public DecisionStatus Decision { get; set; }

    public Group Group { get; set; } = null!;
    public Supervisor Supervisor { get; set; } = null!;
}
