using FUC.Data.Abstractions;
using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Data;

public sealed class MajorGroup : Entity, ISoftDelete
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Major> Majors { get; set; } = new List<Major>();
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
