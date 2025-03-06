using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public class FucTaskHistory : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string Content { get; set; }

    public FucTask FucTask { get; set; } = null!;
}
