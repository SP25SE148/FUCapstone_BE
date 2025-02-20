using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class TechnicalArea : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<StudentExpertise> StudentExpertises { get; set; } = new List<StudentExpertise>();
}
