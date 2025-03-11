using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class ReviewCriteria : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public string CapstoneId { get; set; }
    public int Attempt { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Requirement { get; set; }
    public bool IsActive { get; set; }
    public Capstone Capstone { get; set; } = null!;
}
