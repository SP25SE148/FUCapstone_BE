using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Abstractions;
public abstract class AuditableEntity : Entity, IAuditableEntity
{
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}
