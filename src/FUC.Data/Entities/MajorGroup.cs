using FUC.Data.Abstractions;
using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Entities;


public sealed class MajorGroup : AuditableSoftDeleteEntity
{
    public string Id { get; set; } // major group code
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Major> Majors { get; set; } = new List<Major>();
}
