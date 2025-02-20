using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class BusinessArea : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
