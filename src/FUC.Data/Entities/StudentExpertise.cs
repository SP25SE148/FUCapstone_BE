using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class StudentExpertise : AuditableEntity
{
    public Guid Id { get; set; }
    public string StudentId { get; set; }
    public Guid TechnicalAreaId { get; set; }

    public Student Student { get; set; } = null!;
    public TechnicalArea TechnicalArea { get; set; } = null!;
    
}
